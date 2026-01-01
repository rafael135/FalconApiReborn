using Falcon.Api.Features.Exercises.Shared;

namespace Falcon.Api.Features.Exercises.CreateExercise;

/// <summary>
/// Result of creating an exercise.
/// </summary>
public record CreateExerciseResult(ExerciseDto Exercise);
