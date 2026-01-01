namespace Falcon.Api.Features.Exercises.Shared;

/// <summary>
/// Basic exercise information.
/// </summary>
public record ExerciseDto(
    Guid Id,
    string Title,
    string? Description,
    TimeSpan EstimatedTime,
    int ExerciseTypeId,
    string ExerciseType,
    Guid? AttachedFileId
);
