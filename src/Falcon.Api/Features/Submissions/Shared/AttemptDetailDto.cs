using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Api.Features.Submissions.Shared;

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
