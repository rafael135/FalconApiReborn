using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.StartCompetition;

/// <summary>
/// Result of starting a competition.
/// </summary>
/// <param name="Competition">The competition data after starting.</param>
public record StartCompetitionResult(CompetitionDto Competition);
