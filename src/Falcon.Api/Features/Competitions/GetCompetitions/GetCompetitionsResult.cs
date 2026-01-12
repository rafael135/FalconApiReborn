using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.GetCompetitions;

/// <summary>
/// Result containing a paginated list of competitions.
/// </summary>
/// <param name="Competitions">The list of competitions in the current page.</param>
/// <param name="TotalCount">The total number of competitions matching the filter.</param>
/// <param name="Skip">The offset used for pagination.</param>
/// <param name="Take">The page size used for pagination.</param>
public record GetCompetitionsResult(
    List<CompetitionSummaryDto> Competitions,
    int TotalCount,
    int Skip,
    int Take
);
