using Falcon.Api.Features.Exercises.Shared;

namespace Falcon.Api.Features.Exercises.GetExercises;

/// <summary>
/// Result containing paginated list of exercises.
/// </summary>
public record GetExercisesResult(
    List<ExerciseSummaryDto> Exercises,
    int TotalCount,
    int Skip,
    int Take
);
