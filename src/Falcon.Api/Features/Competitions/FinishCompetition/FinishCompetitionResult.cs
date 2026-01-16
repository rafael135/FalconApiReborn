using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.FinishCompetition;

/// <summary>
/// Result of finishing a competition.
/// </summary>
/// <param name="Competition">The competition data after finishing.</param>
public record FinishCompetitionResult(CompetitionDto Competition);
