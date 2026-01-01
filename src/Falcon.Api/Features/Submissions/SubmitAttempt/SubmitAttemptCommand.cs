using Falcon.Api.Features.Competitions.Shared;
using Falcon.Api.Features.Submissions.Shared;
using Falcon.Core.Domain.Shared.Enums;
using MediatR;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

public record SubmitAttemptCommand(
    Guid CompetitionId,
    Guid ExerciseId,
    string Code,
    LanguageType Language
) : IRequest<SubmitAttemptResult>;
