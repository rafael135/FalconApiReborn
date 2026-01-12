using Falcon.Api.Features.Competitions.Shared;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.GetCompetitions;

/// <summary>
/// Handler for getting competitions with optional filtering and pagination.
/// </summary>
public class GetCompetitionsHandler : IRequestHandler<GetCompetitionsQuery, GetCompetitionsResult>
{
    private readonly FalconDbContext _dbContext;

    public GetCompetitionsHandler(FalconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the <see cref="GetCompetitionsQuery"/> and returns a paginated result of competitions.
    /// </summary>
    /// <param name="request">Query containing filter and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="GetCompetitionsResult"/> containing the competitions page and pagination metadata.</returns>
    public async Task<GetCompetitionsResult> Handle(GetCompetitionsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Competitions.AsNoTracking();

        // Filter by status if provided
        if (request.Status.HasValue)
        {
            query = query.Where(c => c.Status == request.Status.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var competitions = await query
            .OrderByDescending(c => c.StartTime)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(c => new CompetitionSummaryDto(
                c.Id,
                c.Name,
                c.Status,
                c.StartTime,
                c.EndTime,
                c.GroupsInCompetitions.Count
            ))
            .ToListAsync(cancellationToken);

        return new GetCompetitionsResult(
            competitions,
            totalCount,
            request.Skip,
            request.Take
        );
    }
}
