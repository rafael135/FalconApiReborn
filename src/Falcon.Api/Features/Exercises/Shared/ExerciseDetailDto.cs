namespace Falcon.Api.Features.Exercises.Shared;

/// <summary>
/// Detailed exercise information including test cases (for Teachers/Admins).
/// </summary>
public record ExerciseDetailDto(
    Guid Id,
    string Title,
    string? Description,
    TimeSpan EstimatedTime,
    string ExerciseType,
    AttachedFileDto? AttachedFile,
    List<TestCaseDto>? TestCases // null for Students
);
