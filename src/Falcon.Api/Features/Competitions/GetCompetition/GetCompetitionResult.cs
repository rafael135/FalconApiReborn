using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.GetCompetition;

/// <summary>
/// Result containing detailed competition information.
/// </summary>
/// <param name="Competition">Detailed competition DTO.</param>
public record GetCompetitionResult(CompetitionDetailDto Competition);
