using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.GetRanking;

/// <summary>
/// Result containing competition ranking.
/// </summary>
public record GetRankingResult(List<RankingEntryDto> Rankings);
