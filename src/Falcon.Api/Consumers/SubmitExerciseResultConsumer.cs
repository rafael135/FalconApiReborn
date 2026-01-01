using Falcon.Api.Features.Submissions.Shared;
using Falcon.Api.Hubs;
using Falcon.Core.Messages;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace Falcon.Api.Consumers;

/// <summary>
/// Consumes exercise submission results from Worker and sends back to client via SignalR.
/// </summary>
public class SubmitExerciseResultConsumer : IConsumer<ISubmitExerciseResult>
{
    private readonly IHubContext<CompetitionHub> _hubContext;
    private readonly ILogger<SubmitExerciseResultConsumer> _logger;

    public SubmitExerciseResultConsumer(
        IHubContext<CompetitionHub> hubContext,
        ILogger<SubmitExerciseResultConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ISubmitExerciseResult> context)
    {
        var result = context.Message;

        _logger.LogInformation(
            "Received submission result for ConnectionId {ConnectionId}, Correlation {CorrelationId}, Success: {Success}", 
            result.ConnectionId, result.CorrelationId, result.Success);

        if (result.Success)
        {
            // Send success response with attempt and ranking details
            await _hubContext.Clients.Client(result.ConnectionId).SendAsync(
                "ReceiveExerciseAttemptResponse",
                new
                {
                    correlationId = result.CorrelationId,
                    success = true,
                    attempt = new AttemptDto(
                        result.AttemptId!.Value,
                        Guid.Empty, // ExerciseId - not needed here
                        string.Empty, // ExerciseTitle - not needed here
                        Guid.Empty, // GroupId - not needed here
                        string.Empty, // GroupName - not needed here
                        DateTime.UtcNow,
                        Core.Domain.Shared.Enums.LanguageType.CSharp, // Placeholder
                        result.Accepted,
                        result.JudgeResponse,
                        result.ExecutionTime
                    ),
                    ranking = new
                    {
                        rankOrder = result.RankOrder,
                        points = result.Points,
                        penalty = result.Penalty,
                        solvedExercises = result.SolvedExercises
                    }
                });

            // Broadcast ranking update to all clients
            await _hubContext.Clients.All.SendAsync(
                "ReceiveRankingUpdate",
                new
                {
                    rankOrder = result.RankOrder,
                    points = result.Points,
                    penalty = result.Penalty,
                    solvedExercises = result.SolvedExercises,
                    timestamp = DateTime.UtcNow
                });

            _logger.LogInformation(
                "Submission result sent to client {ConnectionId}, Accepted: {Accepted}", 
                result.ConnectionId, result.Accepted);
        }
        else
        {
            // Send error response
            await _hubContext.Clients.Client(result.ConnectionId).SendAsync(
                "ReceiveExerciseAttemptError",
                new
                {
                    correlationId = result.CorrelationId,
                    message = result.ErrorMessage ?? "Erro desconhecido ao processar submissão"
                });

            _logger.LogWarning(
                "Submission failed for ConnectionId {ConnectionId}: {Error}", 
                result.ConnectionId, result.ErrorMessage);
        }
    }
}
