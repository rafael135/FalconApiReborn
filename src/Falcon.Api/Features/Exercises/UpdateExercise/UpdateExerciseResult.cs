using Falcon.Api.Features.Exercises.Shared;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

/// <summary>
/// Resultado retornado após atualização de um exercício.
/// </summary>
public record UpdateExerciseResult(ExerciseDto Exercise);
