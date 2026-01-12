using MediatR;

namespace Falcon.Api.Features.Competitions.GetRanking;

/// <summary>
/// Query to get competition ranking.
/// </summary>
/// <param name="CompetitionId">The competition identifier.</param>
public record GetRankingQuery(Guid CompetitionId) : IRequest<GetRankingResult>; 
