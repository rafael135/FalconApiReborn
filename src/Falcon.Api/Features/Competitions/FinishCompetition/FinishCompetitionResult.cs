using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.FinishCompetition;

/// <summary>
/// Result of finishing a competition.
/// </summary>
public record FinishCompetitionResult(CompetitionDto Competition);
