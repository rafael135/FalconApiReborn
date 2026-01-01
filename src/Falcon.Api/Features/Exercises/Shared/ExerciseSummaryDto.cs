namespace Falcon.Api.Features.Exercises.Shared;

/// <summary>
/// Summary information about an exercise.
/// </summary>
public record ExerciseSummaryDto(
    Guid Id,
    string Title,
    TimeSpan EstimatedTime,
    string ExerciseType
);
