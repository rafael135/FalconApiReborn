using MediatR;

namespace Falcon.Api.Features.Competitions.AddExercise;

public record AddExerciseCommand(Guid CompetitionId, Guid ExerciseId) : IRequest<AddExerciseResult>;
