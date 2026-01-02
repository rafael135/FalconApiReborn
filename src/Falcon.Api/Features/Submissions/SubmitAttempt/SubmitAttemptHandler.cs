using Falcon.Api.Features.Competitions.Shared;
using Falcon.Api.Features.Submissions.Shared;
using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Users;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

public class SubmitAttemptHandler : IRequestHandler<SubmitAttemptCommand, SubmitAttemptResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<SubmitAttemptHandler> _logger;
    private readonly UserManager<User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IJudgeService _judgeService;

    public SubmitAttemptHandler(
        FalconDbContext dbContext,
        ILogger<SubmitAttemptHandler> logger,
        UserManager<User> userManager,
        IHttpContextAccessor httpContextAccessor,
        IJudgeService judgeService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _judgeService = judgeService;
    }

    public async Task<SubmitAttemptResult> Handle(SubmitAttemptCommand request, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string>();
        
        if (string.IsNullOrWhiteSpace(request.Code))
            errors.Add(nameof(request.Code), "Código é obrigatório");
        
        if (errors.Any())
            throw new FormException(errors);

        var userName = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (string.IsNullOrEmpty(userName))
            throw new UnauthorizedAccessException("Usuário não autenticado");

        var user = await _userManager.Users
            .Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

        if (user?.GroupId == null)
        {
            errors.Add("group", "Você deve estar em um grupo para submeter exercícios");
            throw new FormException(errors);
        }

        var groupInCompetition = await _dbContext.GroupsInCompetitions
            .FirstOrDefaultAsync(g => g.GroupId == user.GroupId && g.CompetitionId == request.CompetitionId, cancellationToken);

        if (groupInCompetition == null)
        {
            errors.Add("competition", "Seu grupo não está registrado nesta competição");
            throw new FormException(errors);
        }

        if (groupInCompetition.Blocked)
        {
            errors.Add("group", "Seu grupo foi bloqueado nesta competição");
            throw new FormException(errors);
        }

        var exerciseInCompetition = await _dbContext.ExercisesInCompetition
            .FirstOrDefaultAsync(e => e.CompetitionId == request.CompetitionId && e.ExerciseId == request.ExerciseId, cancellationToken);

        if (exerciseInCompetition == null)
        {
            errors.Add("exercise", "Este exercício não está nesta competição");
            throw new FormException(errors);
        }

        var competition = await _dbContext.Competitions
            .FindAsync(new object[] { request.CompetitionId }, cancellationToken);

        if (competition?.Status != CompetitionStatus.Ongoing)
        {
            errors.Add("competition", "A competição não está em andamento");
            throw new FormException(errors);
        }

        if (competition.BlockSubmissions.HasValue && DateTime.UtcNow > competition.BlockSubmissions.Value)
        {
            errors.Add("competition", "O período de submissões foi encerrado");
            throw new FormException(errors);
        }

        var exercise = await _dbContext.Exercises.FindAsync(new object[] { request.ExerciseId }, cancellationToken);
        if (exercise == null)
            throw new NotFoundException("Exercise", request.ExerciseId);

        if (string.IsNullOrEmpty(exercise.JudgeUuid))
        {
            errors.Add("exercise", "Exercício não está configurado para avaliação");
            throw new FormException(errors);
        }

        var judgeResult = await _judgeService.SubmitCodeAsync(request.Code, request.Language.ToString(), exercise.JudgeUuid);

        var attempt = new GroupExerciseAttempt(exercise, user.Group!, competition, request.Code, request.Language);
        var domainStatus = (Core.Domain.Shared.Enums.JudgeSubmissionResponse)(int)judgeResult.Status;
        attempt.SetJudgeResponse(domainStatus, judgeResult.ExecutionTime);

        await _dbContext.GroupExerciseAttempts.AddAsync(attempt, cancellationToken);

        if (attempt.Accepted)
        {
            var ranking = await _dbContext.CompetitionRankings
                .FirstOrDefaultAsync(r => r.CompetitionId == request.CompetitionId && r.GroupId == user.GroupId, cancellationToken);

            if (ranking == null)
            {
                ranking = new CompetitionRanking(competition, user.Group!);
                await _dbContext.CompetitionRankings.AddAsync(ranking, cancellationToken);
            }

            var alreadySolved = await _dbContext.GroupExerciseAttempts
                .AnyAsync(a => a.GroupId == user.GroupId 
                    && a.ExerciseId == request.ExerciseId 
                    && a.CompetitionId == request.CompetitionId 
                    && a.Accepted 
                    && a.Id != attempt.Id, cancellationToken);

            if (!alreadySolved)
            {
                ranking.UpdatePoints(ranking.Points + 1);

                if (competition.BlockSubmissions.HasValue && DateTime.UtcNow > competition.BlockSubmissions.Value 
                    && competition.StopRanking.HasValue && DateTime.UtcNow < competition.StopRanking.Value)
                {
                    var penaltyValue = competition.SubmissionPenalty?.TotalMinutes ?? 0;
                    ranking.AddPenalty(penaltyValue);
                }
            }
        }

        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
        var log = new Log(LogType.SubmitExercise, ipAddress, user, user.Group, competition);
        await _dbContext.Logs.AddAsync(log, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        await RecalculateRankOrderAsync(request.CompetitionId, cancellationToken);

        _logger.LogInformation("Submission {AttemptId} created for group {GroupId} in competition {CompetitionId}", attempt.Id, user.GroupId, request.CompetitionId);

        var attemptDto = new AttemptDto(
            attempt.Id, exercise.Id, exercise.Title, user.GroupId.Value, user.Group!.Name,
            attempt.SubmissionTime, attempt.Language, attempt.Accepted, attempt.JudgeResponse, attempt.Time
        );

        var updatedRanking = await _dbContext.CompetitionRankings
            .Include(r => r.Group)
            .FirstOrDefaultAsync(r => r.CompetitionId == request.CompetitionId && r.GroupId == user.GroupId, cancellationToken);

        var solvedCount = await _dbContext.GroupExerciseAttempts
            .Where(a => a.GroupId == user.GroupId && a.CompetitionId == request.CompetitionId && a.Accepted)
            .Select(a => a.ExerciseId)
            .Distinct()
            .CountAsync(cancellationToken);

        var rankingDto = new RankingEntryDto(
            updatedRanking!.GroupId, updatedRanking.Group.Name,
            updatedRanking.Points, updatedRanking.Penalty, updatedRanking.RankOrder,
            solvedCount, attempt.SubmissionTime
        );

        return new SubmitAttemptResult(attemptDto, rankingDto);
    }

    private async Task RecalculateRankOrderAsync(Guid competitionId, CancellationToken cancellationToken)
    {
        var rankings = await _dbContext.CompetitionRankings
            .Where(r => r.CompetitionId == competitionId)
            .OrderByDescending(r => r.Points)
            .ThenBy(r => r.Penalty)
            .ToListAsync(cancellationToken);

        int order = 1;
        foreach (var ranking in rankings)
        {
            ranking.UpdateRankOrder(order++);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
