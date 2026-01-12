using Falcon.Api.Features.Exercises.Shared;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

/// <summary>
/// Result returned after updating an exercise.
/// </summary>
public record UpdateExerciseResult(ExerciseDto Exercise);
