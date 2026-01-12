using Falcon.Api.Features.Competitions.Shared;
using Falcon.Api.Features.Exercises.Shared;
using Falcon.Api.Features.Questions.Shared;
using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Users;
using Falcon.Core.Messages;
using Falcon.Infrastructure.Database;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.Hubs;

/// <summary>
/// SignalR hub for real-time competition operations.
/// </summary>
[Authorize]
public class CompetitionHub : Hub
{
    private readonly FalconDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CompetitionHub> _logger;

    /// <summary>
    /// Creates a new instance of <see cref="CompetitionHub"/>.
    /// </summary>
    /// <param name="dbContext">Database context for reads/writes.</param>
    /// <param name="userManager">Identity user manager to resolve user information and roles.</param>
    /// <param name="publishEndpoint">MassTransit publish endpoint used to publish submission commands to the worker.</param>
    /// <param name="logger">Logger instance.</param>
    public CompetitionHub(
        FalconDbContext dbContext,
        UserManager<User> userManager,
        IPublishEndpoint publishEndpoint,
        ILogger<CompetitionHub> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    /// <summary>
    /// Called when a client connects to the hub; verifies identity and assigns the connection to role/group groups.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userName = Context.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            Context.Abort();
            return;
        }

        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
        {
            Context.Abort();
            return;
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Contains("Admin"))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
        
        if (roles.Contains("Teacher"))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Teachers");
        }
        
        if (roles.Contains("Student"))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Students");
            if (user.GroupId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Group:{user.GroupId.Value}");
            }
        }

        _logger.LogInformation("User {UserName} connected to CompetitionHub with ConnectionId {ConnectionId}", 
            userName, Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Submits an exercise solution for evaluation. Publishes to RabbitMQ for async processing.
    /// </summary>
    [Authorize(Roles = "Student")]
    public async Task SendExerciseAttempt(Guid competitionId, Guid exerciseId, string code, int languageType)
    {
        var userName = Context.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            await Clients.Caller.SendAsync("ReceiveExerciseAttemptError", 
                new { message = "Usuário não autenticado" });
            return;
        }

        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName);

        if (user?.GroupId == null)
        {
            await Clients.Caller.SendAsync("ReceiveExerciseAttemptError", 
                new { message = "Você deve estar em um grupo para submeter exercícios" });
            return;
        }

        // Check if group is blocked
        var groupInCompetition = await _dbContext.GroupsInCompetitions
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.GroupId == user.GroupId && g.CompetitionId == competitionId);

        if (groupInCompetition == null)
        {
            await Clients.Caller.SendAsync("ReceiveExerciseAttemptError", 
                new { message = "Seu grupo não está registrado nesta competição" });
            return;
        }

        if (groupInCompetition.Blocked)
        {
            await Clients.Caller.SendAsync("ReceiveExerciseAttemptError", 
                new { message = "Seu grupo foi bloqueado nesta competição" });
            return;
        }

        // Check if exercise is in competition
        var exerciseInCompetition = await _dbContext.ExercisesInCompetition
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.CompetitionId == competitionId && e.ExerciseId == exerciseId);

        if (exerciseInCompetition == null)
        {
            await Clients.Caller.SendAsync("ReceiveExerciseAttemptError", 
                new { message = "Este exercício não está nesta competição" });
            return;
        }

        // Check if already accepted
        var alreadyAccepted = await _dbContext.GroupExerciseAttempts
            .AnyAsync(a => a.GroupId == user.GroupId 
                && a.ExerciseId == exerciseId 
                && a.CompetitionId == competitionId 
                && a.Accepted);

        if (alreadyAccepted)
        {
            await Clients.Caller.SendAsync("ReceiveExerciseAttemptError", 
                new { message = "Este exercício já foi aceito pelo seu grupo" });
            return;
        }

        // Publish to RabbitMQ
        var correlationId = Guid.NewGuid();
        await _publishEndpoint.Publish<ISubmitExerciseCommand>(new
        {
            CorrelationId = correlationId,
            ConnectionId = Context.ConnectionId,
            GroupId = user.GroupId.Value,
            ExerciseId = exerciseId,
            CompetitionId = competitionId,
            Code = code,
            Language = (Core.Domain.Shared.Enums.LanguageType)languageType,
            SubmittedAt = DateTime.UtcNow
        });

        _logger.LogInformation("Exercise submission published to queue: Group {GroupId}, Exercise {ExerciseId}, Correlation {CorrelationId}", 
            user.GroupId.Value, exerciseId, correlationId);

        await Clients.Caller.SendAsync("ReceiveExerciseAttemptQueued", 
            new { correlationId, message = "Submissão enviada para avaliação" });
    }

    /// <summary>
    /// Sends real-time ranking update to all connected clients.
    /// </summary>
    public async Task SendRankingUpdate(Guid competitionId, List<RankingEntryDto> rankings)
    {
        await Clients.All.SendAsync("ReceiveRankingUpdate", rankings);
    }

    /// <summary>
    /// Gets current competition details for initial connection.
    /// </summary>
    public async Task GetCurrentCompetition()
    {
        var competition = await _dbContext.Competitions
            .AsNoTracking()
            .Include(c => c.Rankings)
                .ThenInclude(r => r.Group)
                    .ThenInclude(g => g.Users)
            .Include(c => c.ExercisesInCompetition)
                .ThenInclude(ec => ec.Exercise)
                    .ThenInclude(e => e.ExerciseType)
            .Where(c => c.Status == Core.Domain.Competitions.CompetitionStatus.Ongoing)
            .OrderByDescending(c => c.StartTime)
            .FirstOrDefaultAsync();

        if (competition == null)
        {
            await Clients.Caller.SendAsync("OnConnectionResponse", null);
            return;
        }

        var userName = Context.User?.Identity?.Name;
        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName);

        var response = new CompetitionDetailDto(
            competition.Id,
            competition.Name,
            competition.Description ?? string.Empty,
            competition.Status,
            competition.StartInscriptions,
            competition.EndInscriptions,
            competition.StartTime,
            competition.EndTime,
            competition.MaxMembers,
            competition.MaxExercises,
            competition.MaxSubmissionSize,
            competition.Duration,
            competition.SubmissionPenalty,
            competition.ExercisesInCompetition
                .Select(ec => new ExerciseSummaryDto(
                    ec.Exercise.Id, 
                    ec.Exercise.Title, 
                    ec.Exercise.EstimatedTime,
                    ec.Exercise.ExerciseType.Label
                ))
                .ToList(),
            competition.Rankings
                .OrderBy(r => r.RankOrder)
                .Select(r => new RankingEntryDto(
                    r.GroupId, r.Group.Name, r.Points, r.Penalty, r.RankOrder,
                    0, // SolvedExercises - calculate if needed
                    null // LastSubmissionTime - calculate if needed
                ))
                .ToList()
        );

        await Clients.Caller.SendAsync("OnConnectionResponse", response);
    }

    /// <summary>
    /// Simple ping method to check connectivity; replies with a Pong message and timestamp.
    /// </summary>
    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", new { message = "Pong", timestamp = DateTime.UtcNow });
    }

    /// <summary>
    /// Asks a question in the competition (Students only).
    /// </summary>
    [Authorize(Roles = "Student")]
    public async Task AskQuestion(Guid competitionId, Guid? exerciseId, string content, int questionType)
    {
        var userName = Context.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            await Clients.Caller.SendAsync("ReceiveQuestionError", 
                new { message = "Usuário não autenticado" });
            return;
        }

        var user = await _userManager.Users
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.UserName == userName);

        if (user?.GroupId == null)
        {
            await Clients.Caller.SendAsync("ReceiveQuestionError", 
                new { message = "Você deve estar em um grupo para fazer perguntas" });
            return;
        }

        var competition = await _dbContext.Competitions
            .FirstOrDefaultAsync(c => c.Id == competitionId);

        if (competition == null)
        {
            await Clients.Caller.SendAsync("ReceiveQuestionError", 
                new { message = "Competição não encontrada" });
            return;
        }

        Exercise? exercise = null;
        if (exerciseId.HasValue)
        {
            exercise = await _dbContext.Exercises
                .FirstOrDefaultAsync(e => e.Id == exerciseId.Value);
        }

        var question = new Question(
            competition,
            user,
            content,
            (QuestionType)questionType,
            exercise
        );

        await _dbContext.Questions.AddAsync(question);

        // Create log
        var ipAddress = Context.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
        var log = new Log(LogType.QuestionSent, ipAddress, user, user.Group, competition);
        await _dbContext.Logs.AddAsync(log);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Question {QuestionId} created by user {UserId} in competition {CompetitionId}", 
            question.Id, user.Id, competitionId);

        var questionDto = new QuestionDto(
            question.Id,
            question.CompetitionId,
            question.ExerciseId,
            user.Id,
            user.Name,
            user.GroupId,
            user.Group?.Name,
            question.Content,
            question.QuestionType,
            question.CreatedAt,
            null
        );

        // Send to caller
        await Clients.Caller.SendAsync("ReceiveQuestionCreated", questionDto);

        // Broadcast to Teachers and Admins
        await Clients.Group("Teachers").SendAsync("ReceiveNewQuestion", questionDto);
        await Clients.Group("Admins").SendAsync("ReceiveNewQuestion", questionDto);
    }

    /// <summary>
    /// Answers a question (Teachers/Admins only).
    /// </summary>
    [Authorize(Roles = "Teacher,Admin")]
    public async Task AnswerQuestion(Guid questionId, string content)
    {
        var userName = Context.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            await Clients.Caller.SendAsync("ReceiveAnswerError", 
                new { message = "Usuário não autenticado" });
            return;
        }

        var user = await _userManager.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
        {
            await Clients.Caller.SendAsync("ReceiveAnswerError", 
                new { message = "Usuário não encontrado" });
            return;
        }

        var question = await _dbContext.Questions
            .Include(q => q.User)
                .ThenInclude(u => u.Group)
            .Include(q => q.Competition)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question == null)
        {
            await Clients.Caller.SendAsync("ReceiveAnswerError", 
                new { message = "Pergunta não encontrada" });
            return;
        }

        if (question.Answer != null)
        {
            await Clients.Caller.SendAsync("ReceiveAnswerError", 
                new { message = "Pergunta já foi respondida" });
            return;
        }

        var answer = new Answer(user, content);
        await _dbContext.Answers.AddAsync(answer);
        
        question.SetAnswer(answer);

        // Create log
        var ipAddress = Context.GetHttpContext()?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
        var log = new Log(LogType.AnswerGiven, ipAddress, user, null, question.Competition);
        await _dbContext.Logs.AddAsync(log);

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Answer {AnswerId} created for question {QuestionId} by user {UserId}", 
            answer.Id, questionId, user.Id);

        var answerDto = new AnswerDto(
            answer.Id,
            answer.Content,
            user.Id,
            user.Name,
            answer.CreatedAt
        );

        var questionDto = new QuestionDto(
            question.Id,
            question.CompetitionId,
            question.ExerciseId,
            question.User.Id,
            question.User.Name,
            question.User.GroupId,
            question.User.Group?.Name,
            question.Content,
            question.QuestionType,
            question.CreatedAt,
            answerDto
        );

        // Send to caller
        await Clients.Caller.SendAsync("ReceiveAnswerCreated", questionDto);

        // Broadcast to all connected clients
        await Clients.All.SendAsync("ReceiveQuestionAnswered", questionDto);

        // Notify the question author's group
        if (question.User.GroupId.HasValue)
        {
            await Clients.Group($"Group:{question.User.GroupId.Value}")
                .SendAsync("ReceiveYourQuestionAnswered", questionDto);
        }
    }

    /// <summary>
    /// Updates an existing answer (Teachers/Admins only).
    /// </summary>
    [Authorize(Roles = "Teacher,Admin")]
    public async Task UpdateAnswer(Guid answerId, string content)
    {
        var userName = Context.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
        {
            await Clients.Caller.SendAsync("ReceiveAnswerUpdateError", 
                new { message = "Usuário não autenticado" });
            return;
        }

        var answer = await _dbContext.Answers
            .Include(a => a.User)
            .Include(a => a.Question)
                .ThenInclude(q => q!.User)
                    .ThenInclude(u => u.Group)
            .Include(a => a.Question)
                .ThenInclude(q => q!.Competition)
            .FirstOrDefaultAsync(a => a.Id == answerId);

        if (answer == null)
        {
            await Clients.Caller.SendAsync("ReceiveAnswerUpdateError", 
                new { message = "Resposta não encontrada" });
            return;
        }

        answer.UpdateContent(content);
        await _dbContext.SaveChangesAsync();

        // Create audit log
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userName);
        if (user != null)
        {
            var ipAddress = Context.GetHttpContext()?.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var log = new Log(
                LogType.AnswerUpdated,
                ipAddress,
                user,
                user.Group,
                answer.Question!.Competition
            );
            await _dbContext.Logs.AddAsync(log);
            await _dbContext.SaveChangesAsync();
        }

        _logger.LogInformation("Answer {AnswerId} updated by user {UserId}", answerId, userName);

        var answerDto = new AnswerDto(
            answer.Id,
            answer.Content,
            answer.User.Id,
            answer.User.Name,
            answer.CreatedAt
        );

        var questionDto = new QuestionDto(
            answer.Question!.Id,
            answer.Question.CompetitionId,
            answer.Question.ExerciseId,
            answer.Question.User.Id,
            answer.Question.User.Name,
            answer.Question.User.GroupId,
            answer.Question.User.Group?.Name,
            answer.Question.Content,
            answer.Question.QuestionType,
            answer.Question.CreatedAt,
            answerDto
        );

        // Send to caller
        await Clients.Caller.SendAsync("ReceiveAnswerUpdated", questionDto);

        // Broadcast update to all clients
        await Clients.All.SendAsync("ReceiveQuestionAnswered", questionDto);
    }

    /// <summary>
    /// Gets all questions for the current competition.
    /// </summary>
    [Authorize]
    public async Task GetAllQuestions(Guid competitionId)
    {
        var questions = await _dbContext.Questions
            .AsNoTracking()
            .Include(q => q.User)
                .ThenInclude(u => u.Group)
            .Include(q => q.Answer)
                .ThenInclude(a => a!.User)
            .Where(q => q.CompetitionId == competitionId)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuestionDto(
                q.Id,
                q.CompetitionId,
                q.ExerciseId,
                q.User.Id,
                q.User.Name,
                q.User.GroupId,
                q.User.Group != null ? q.User.Group.Name : null,
                q.Content,
                q.QuestionType,
                q.CreatedAt,
                q.Answer != null ? new AnswerDto(
                    q.Answer.Id,
                    q.Answer.Content,
                    q.Answer.User.Id,
                    q.Answer.User.Name,
                    q.Answer.CreatedAt
                ) : null
            ))
            .ToListAsync();

        await Clients.Caller.SendAsync("ReceiveAllQuestions", questions);
    }
}
