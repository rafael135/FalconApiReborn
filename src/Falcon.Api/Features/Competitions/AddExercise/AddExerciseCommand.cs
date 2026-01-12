using MediatR;

namespace Falcon.Api.Features.Competitions.AddExercise;

/// <summary>
/// Command to add an exercise to a competition.
/// </summary>
/// <param name="CompetitionId">The target competition identifier.</param>
/// <param name="ExerciseId">The exercise identifier to add.</param>
public record AddExerciseCommand(Guid CompetitionId, Guid ExerciseId) : IRequest<AddExerciseResult>;
