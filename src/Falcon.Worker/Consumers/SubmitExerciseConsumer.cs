using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Interfaces;
using Falcon.Core.Messages;
using Falcon.Infrastructure.Database;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DomainJudgeResponse = Falcon.Core.Domain.Shared.Enums.JudgeSubmissionResponse;

namespace Falcon.Worker.Consumers;

/// <summary>
/// Consumes exercise submission commands from RabbitMQ and processes them.
/// </summary>
public class SubmitExerciseConsumer : IConsumer<ISubmitExerciseCommand>
{
    private readonly FalconDbContext _dbContext;
    private readonly IJudgeService _judgeService;
    private readonly ILogger<SubmitExerciseConsumer> _logger;

    public SubmitExerciseConsumer(
        FalconDbContext dbContext,
        IJudgeService judgeService,
        ILogger<SubmitExerciseConsumer> logger)
    {
        _dbContext = dbContext;
        _judgeService = judgeService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ISubmitExerciseCommand> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("Processing exercise submission: Correlation {CorrelationId}, Group {GroupId}, Exercise {ExerciseId}", 
            message.CorrelationId, message.GroupId, message.ExerciseId);

        try
        {
            // Load entities
            var exercise = await _dbContext.Exercises
                .FirstOrDefaultAsync(e => e.Id == message.ExerciseId);

            var group = await _dbContext.Groups
                .FirstOrDefaultAsync(g => g.Id == message.GroupId);

            var competition = await _dbContext.Competitions
                .FirstOrDefaultAsync(c => c.Id == message.CompetitionId);

            if (exercise == null || group == null || competition == null)
            {
                await context.Publish<ISubmitExerciseResult>(new
                {
                    message.CorrelationId,
                    message.ConnectionId,
                    Success = false,
                    ErrorMessage = "Entidades não encontradas",
                    AttemptId = (Guid?)null,
                    Accepted = false,
                    JudgeResponse = DomainJudgeResponse.CompilationError,
                    ExecutionTime = TimeSpan.Zero,
                    RankOrder = 0,
                    Points = 0.0,
                    Penalty = 0.0,
                    SolvedExercises = 0
                });
                return;
            }

            if (string.IsNullOrEmpty(exercise.JudgeUuid))
            {
                await context.Publish<ISubmitExerciseResult>(new
                {
                    message.CorrelationId,
                    message.ConnectionId,
                    Success = false,
                    ErrorMessage = "Exercício não configurado para avaliação",
                    AttemptId = (Guid?)null,
                    Accepted = false,
                    JudgeResponse = DomainJudgeResponse.CompilationError,
                    ExecutionTime = TimeSpan.Zero,
                    RankOrder = 0,
                    Points = 0.0,
                    Penalty = 0.0,
                    SolvedExercises = 0
                });
                return;
            }

            // Call Judge Service
            var judgeResult = await _judgeService.SubmitCodeAsync(
                message.Code, 
                message.Language.ToString(), 
                exercise.JudgeUuid);

            // Create attempt
            var attempt = new GroupExerciseAttempt(exercise, group, competition, message.Code, message.Language);
            var domainStatus = (DomainJudgeResponse)(int)judgeResult.Status;
            attempt.SetJudgeResponse(domainStatus, judgeResult.ExecutionTime);

            await _dbContext.GroupExerciseAttempts.AddAsync(attempt);

            // Update ranking if accepted
            double points = 0;
            double penalty = 0;
            int rankOrder = 0;

            if (attempt.Accepted)
            {
                var ranking = await _dbContext.CompetitionRankings
                    .FirstOrDefaultAsync(r => r.CompetitionId == message.CompetitionId && r.GroupId == message.GroupId);

                if (ranking == null)
                {
                    ranking = new Core.Domain.Competitions.CompetitionRanking(competition, group);
                    await _dbContext.CompetitionRankings.AddAsync(ranking);
                }

                // Check if already solved before
                var alreadySolved = await _dbContext.GroupExerciseAttempts
                    .AnyAsync(a => a.GroupId == message.GroupId 
                        && a.ExerciseId == message.ExerciseId 
                        && a.CompetitionId == message.CompetitionId 
                        && a.Accepted 
                        && a.Id != attempt.Id);

                if (!alreadySolved)
                {
                    ranking.UpdatePoints(ranking.Points + 100);

                    // Add penalty if after BlockSubmissions
                    if (competition.BlockSubmissions.HasValue && DateTime.UtcNow > competition.BlockSubmissions.Value 
                        && competition.StopRanking.HasValue && DateTime.UtcNow < competition.StopRanking.Value)
                    {
                        var penaltyValue = competition.SubmissionPenalty?.TotalMinutes ?? 0;
                        ranking.AddPenalty(penaltyValue);
                    }
                }

                points = ranking.Points;
                penalty = ranking.Penalty;
            }

            // Create log
            var log = new Log(
                LogType.SubmitExercise, 
                "worker", 
                null, 
                group, 
                competition);
            await _dbContext.Logs.AddAsync(log);

            await _dbContext.SaveChangesAsync();

            // Recalculate rank order
            var rankings = await _dbContext.CompetitionRankings
                .Where(r => r.CompetitionId == message.CompetitionId)
                .OrderByDescending(r => r.Points)
                .ThenBy(r => r.Penalty)
                .ToListAsync();

            int order = 1;
            foreach (var r in rankings)
            {
                r.UpdateRankOrder(order++);
                if (r.GroupId == message.GroupId)
                    rankOrder = r.RankOrder;
            }

            await _dbContext.SaveChangesAsync();

            // Count solved exercises
            var solvedCount = await _dbContext.GroupExerciseAttempts
                .Where(a => a.GroupId == message.GroupId && a.CompetitionId == message.CompetitionId && a.Accepted)
                .Select(a => a.ExerciseId)
                .Distinct()
                .CountAsync();

            // Publish result
            await context.Publish<ISubmitExerciseResult>(new
            {
                message.CorrelationId,
                message.ConnectionId,
                Success = true,
                ErrorMessage = (string?)null,
                AttemptId = (Guid?)attempt.Id,
                attempt.Accepted,
                JudgeResponse = attempt.JudgeResponse,
                ExecutionTime = attempt.Time,
                RankOrder = rankOrder,
                Points = points,
                Penalty = penalty,
                SolvedExercises = solvedCount
            });

            _logger.LogInformation("Exercise submission processed successfully: Correlation {CorrelationId}, Accepted: {Accepted}", 
                message.CorrelationId, attempt.Accepted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing exercise submission: Correlation {CorrelationId}", message.CorrelationId);
            
            await context.Publish<ISubmitExerciseResult>(new
            {
                message.CorrelationId,
                message.ConnectionId,
                Success = false,
                ErrorMessage = $"Erro ao processar submissão: {ex.Message}",
                AttemptId = (Guid?)null,
                Accepted = false,
                JudgeResponse = Core.Domain.Shared.Enums.JudgeSubmissionResponse.RuntimeError,
                ExecutionTime = TimeSpan.Zero,
                RankOrder = 0,
                Points = 0.0,
                Penalty = 0.0,
                SolvedExercises = 0
            });
        }
    }
}
