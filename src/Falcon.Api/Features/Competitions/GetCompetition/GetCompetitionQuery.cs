using MediatR;

namespace Falcon.Api.Features.Competitions.GetCompetition;

/// <summary>
/// Query to get a single competition by ID with full details.
/// </summary>
public record GetCompetitionQuery(Guid CompetitionId) : IRequest<GetCompetitionResult>;
