using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.StartCompetition;

/// <summary>
/// Result of starting a competition.
/// </summary>
public record StartCompetitionResult(CompetitionDto Competition);
