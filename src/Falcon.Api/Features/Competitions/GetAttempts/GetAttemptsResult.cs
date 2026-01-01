using Falcon.Api.Features.Submissions.Shared;

namespace Falcon.Api.Features.Competitions.GetAttempts;

public record GetAttemptsResult(List<AttemptDto> Attempts, int TotalCount);
