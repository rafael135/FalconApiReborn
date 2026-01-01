using Falcon.Api.Features.Competitions.Shared;
using Falcon.Api.Features.Submissions.Shared;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

public record SubmitAttemptResult(AttemptDto Attempt, RankingEntryDto Ranking);
