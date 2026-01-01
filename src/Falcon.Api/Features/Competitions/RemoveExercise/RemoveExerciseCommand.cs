using MediatR;

namespace Falcon.Api.Features.Competitions.RemoveExercise;

public record RemoveExerciseCommand(Guid CompetitionId, Guid ExerciseId) : IRequest<RemoveExerciseResult>;
