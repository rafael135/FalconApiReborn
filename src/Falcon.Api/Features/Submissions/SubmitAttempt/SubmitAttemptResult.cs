using Falcon.Api.Features.Competitions.Shared;
using Falcon.Api.Features.Submissions.Shared;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

/// <summary>
/// Initial result of a submission: the attempt created and its ranking entry.
/// </summary>
/// <param name="Attempt">Details of the created attempt.</param>
/// <param name="Ranking">Relevant ranking entry.</param>
public record SubmitAttemptResult(AttemptDto Attempt, RankingEntryDto Ranking);
