using Falcon.Api.Features.Users.Shared;

namespace Falcon.Api.Features.Users.GetCompetitionHistory;

/// <summary>
/// Result containing the user's competition history with pagination information.
/// </summary>
/// <param name="Items">The list of competition participation records.</param>
/// <param name="Total">The total number of competition participations.</param>
/// <param name="Skip">The number of records skipped.</param>
/// <param name="Take">The number of records taken.</param>
public record GetCompetitionHistoryResult(
    List<CompetitionHistoryDto> Items,
    int Total,
    int Skip,
    int Take
);
