using MediatR;
using Falcon.Core.Domain.Competitions;

namespace Falcon.Api.Features.Competitions.GetCompetitions;

/// <summary>
/// Query to get competitions with optional filtering and pagination.
/// </summary>
public record GetCompetitionsQuery(
    CompetitionStatus? Status = null,
    int Skip = 0,
    int Take = 10
) : IRequest<GetCompetitionsResult>;
