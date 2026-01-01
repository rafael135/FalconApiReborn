using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.GetCompetitions;

/// <summary>
/// Result containing paginated list of competitions.
/// </summary>
public record GetCompetitionsResult(
    List<CompetitionSummaryDto> Competitions,
    int TotalCount,
    int Skip,
    int Take
);
