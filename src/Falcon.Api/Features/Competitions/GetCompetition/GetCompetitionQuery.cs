using MediatR;

namespace Falcon.Api.Features.Competitions.GetCompetition;

/// <summary>
/// Query to get a single competition by ID with full details.
/// </summary>
/// <param name="CompetitionId">The competition identifier.</param>
public record GetCompetitionQuery(Guid CompetitionId) : IRequest<GetCompetitionResult>;
