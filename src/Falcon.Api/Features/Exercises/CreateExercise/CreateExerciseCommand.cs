using MediatR;

namespace Falcon.Api.Features.Exercises.CreateExercise;

/// <summary>
/// Command to create a new exercise.
/// </summary>
public record CreateExerciseCommand(
    string Title,
    string? Description,
    int ExerciseTypeId,
    TimeSpan EstimatedTime
) : IRequest<CreateExerciseResult>;
