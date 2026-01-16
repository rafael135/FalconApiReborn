using Falcon.Core.Domain.Competitions;
using MediatR;

namespace Falcon.Api.Features.Competitions.GetCompetitions;

/// <summary>
/// Query to get competitions with optional filtering and pagination.
/// </summary>
/// <param name="Status">Optional filter by <see cref="CompetitionStatus"/>.</param>
/// <param name="Skip">Number of items to skip (for pagination).</param>
/// <param name="Take">Number of items to take (for pagination).</param>
/// <remarks>
/// The handler will return a paginated result with the total count and the selected page.
/// </remarks>
public record GetCompetitionsQuery(CompetitionStatus? Status = null, int Skip = 0, int Take = 10)
    : IRequest<GetCompetitionsResult>;
