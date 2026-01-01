using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Core.Messages;

/// <summary>
/// Command to submit an exercise solution for evaluation via RabbitMQ.
/// </summary>
public interface ISubmitExerciseCommand
{
    Guid CorrelationId { get; }
    string ConnectionId { get; }
    Guid GroupId { get; }
    Guid ExerciseId { get; }
    Guid CompetitionId { get; }
    string Code { get; }
    LanguageType Language { get; }
    DateTime SubmittedAt { get; }
}
