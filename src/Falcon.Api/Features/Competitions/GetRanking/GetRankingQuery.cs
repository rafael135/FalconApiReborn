using MediatR;

namespace Falcon.Api.Features.Competitions.GetRanking;

/// <summary>
/// Query to get competition ranking.
/// </summary>
public record GetRankingQuery(Guid CompetitionId) : IRequest<GetRankingResult>;
