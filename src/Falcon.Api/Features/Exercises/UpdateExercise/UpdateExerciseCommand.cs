using MediatR;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

public record UpdateExerciseCommand(
    Guid ExerciseId,
    string? Title,
    string? Description,
    int? ExerciseTypeId
) : IRequest<UpdateExerciseResult>;
