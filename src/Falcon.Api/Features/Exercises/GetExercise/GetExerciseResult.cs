using Falcon.Api.Features.Exercises.Shared;

namespace Falcon.Api.Features.Exercises.GetExercise;

/// <summary>
/// Result of getting an exercise.
/// </summary>
public record GetExerciseResult(ExerciseDetailDto Exercise);
