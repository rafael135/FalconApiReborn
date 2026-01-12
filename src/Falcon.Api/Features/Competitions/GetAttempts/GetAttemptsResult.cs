using Falcon.Api.Features.Submissions.Shared;

namespace Falcon.Api.Features.Competitions.GetAttempts;

/// <summary>
/// Result containing attempts for a competition with total count.
/// </summary>
/// <param name="Attempts">List of attempts in the current page.</param>
/// <param name="TotalCount">Total number of attempts matching the filter.</param>
public record GetAttemptsResult(List<AttemptDto> Attempts, int TotalCount);
