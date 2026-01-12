using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Api.Features.Submissions.Shared;

/// <summary>
/// Detailed DTO representing an exercise submission attempt with full code and judge response.
/// </summary>
/// <param name="Id">Attempt unique identifier.</param>
/// <param name="ExerciseId">Associated exercise identifier.</param>
/// <param name="ExerciseTitle">The title of the exercise at the time of submission.</param>
/// <param name="GroupId">Group identifier that made the submission.</param>
/// <param name="GroupName">Group display name.</param>
/// <param name="Code">The submitted source code text.</param>
/// <param name="Language">Programming language used for the attempt.</param>
/// <param name="SubmissionTime">UTC timestamp when the submission was created.</param>
/// <param name="ExecutionTime">Execution time reported by the judge.</param>
/// <param name="Accepted">Whether the submission was accepted by the judge.</param>
/// <param name="JudgeResponse">Raw judge API response object.</param>
public record AttemptDetailDto(
    Guid Id,
    Guid ExerciseId,
    string ExerciseTitle,
    Guid GroupId,
    string GroupName,
    string Code,
    LanguageType Language,
    DateTime SubmissionTime,
    TimeSpan ExecutionTime,
    bool Accepted,
    JudgeSubmissionResponse JudgeResponse
);
