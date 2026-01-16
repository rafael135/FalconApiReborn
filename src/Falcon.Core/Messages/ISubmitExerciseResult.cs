using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Core.Messages;

/// <summary>
/// Result of exercise submission processing from Worker.
/// </summary>
public interface ISubmitExerciseResult
{
    Guid CorrelationId { get; }
    string ConnectionId { get; }
    bool Success { get; }
    string? ErrorMessage { get; }

    // Attempt details
    Guid? AttemptId { get; }
    bool Accepted { get; }
    JudgeSubmissionResponse JudgeResponse { get; }
    TimeSpan ExecutionTime { get; }

    // Ranking details
    int RankOrder { get; }
    double Points { get; }
    double Penalty { get; }
    int SolvedExercises { get; }
}
