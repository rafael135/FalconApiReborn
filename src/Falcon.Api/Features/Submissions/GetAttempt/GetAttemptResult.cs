using Falcon.Api.Features.Submissions.Shared;

namespace Falcon.Api.Features.Submissions.GetAttempt;

/// <summary>
/// Result containing the detailed attempt information.
/// </summary>
/// <param name="Attempt">The detailed attempt dto with code and judge response.</param>
public record GetAttemptResult(AttemptDetailDto Attempt);
