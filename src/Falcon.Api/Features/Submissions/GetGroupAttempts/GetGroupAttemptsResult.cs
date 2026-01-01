using Falcon.Api.Features.Submissions.Shared;

namespace Falcon.Api.Features.Submissions.GetGroupAttempts;

public record GetGroupAttemptsResult(List<AttemptDto> Attempts);
