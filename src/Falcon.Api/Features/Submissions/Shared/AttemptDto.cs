using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Api.Features.Submissions.Shared;

/// <summary>
/// DTO representing a brief attempt summary shown in lists and overviews.
/// </summary>
/// <param name="Id">Attempt unique identifier.</param>
/// <param name="ExerciseId">Associated exercise identifier.</param>
/// <param name="ExerciseTitle">The title of the exercise at the time of submission.</param>
/// <param name="GroupId">Group identifier that made the submission.</param>
/// <param name="GroupName">Group display name.</param>
/// <param name="SubmissionTime">UTC timestamp when the submission was created.</param>
/// <param name="Language">Programming language used for the attempt.</param>
/// <param name="Accepted">Whether the submission was accepted by the judge.</param>
/// <param name="JudgeResponse">Raw judge API response object.</param>
/// <param name="ExecutionTime">Execution time reported by the judge.</param>
public record AttemptDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseTitle,
    Guid GroupId,
    string GroupName,
    DateTime SubmissionTime,
    LanguageType Language,
    bool Accepted,
    JudgeSubmissionResponse JudgeResponse,
    TimeSpan ExecutionTime
);
