using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Api.Features.Submissions.Shared;

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
