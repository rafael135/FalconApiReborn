using Falcon.Api.Features.Submissions.Shared;

namespace Falcon.Api.Features.Submissions.GetGroupAttempts;

/// <summary>
/// Result wrapping a list of attempts from the authenticated user's group.
/// </summary>
/// <param name="Attempts">List of attempt summaries.</param>
public record GetGroupAttemptsResult(List<AttemptDto> Attempts);
