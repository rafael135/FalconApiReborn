using MediatR;

namespace Falcon.Api.Features.Competitions.RemoveExercise;

/// <summary>
/// Command to remove an exercise from a competition.
/// </summary>
/// <param name="CompetitionId">The competition identifier.</param>
/// <param name="ExerciseId">The exercise identifier to remove.</param>
public record RemoveExerciseCommand(Guid CompetitionId, Guid ExerciseId)
    : IRequest<RemoveExerciseResult>;
