using Falcon.Api.Features.Competitions.Shared;
using Falcon.Api.Features.Exercises.Shared;
using Falcon.Core.Domain.Users;
using Falcon.Core.Messages;
using Falcon.Infrastructure.Database;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Hubs;

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

    public async Task Ping()
    {
        await Clients.Caller.SendAsync("Pong", new { message = "Pong", timestamp = DateTime.UtcNow });
    }
}
