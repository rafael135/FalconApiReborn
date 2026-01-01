using Falcon.Api.Features.Competitions.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.GetRanking;

/// <summary>
/// Handler for getting competition ranking.
/// </summary>
public class GetRankingHandler : IRequestHandler<GetRankingQuery, GetRankingResult>
{
    private readonly FalconDbContext _dbContext;

    public GetRankingHandler(FalconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetRankingResult> Handle(GetRankingQuery request, CancellationToken cancellationToken)
    {
        // Verify competition exists
        var competitionExists = await _dbContext.Competitions
            .AnyAsync(c => c.Id == request.CompetitionId, cancellationToken);

        if (!competitionExists)
        {
            throw new NotFoundException("Competition", request.CompetitionId);
        }

        // Get rankings with group information and attempts
        var rankings = await _dbContext.CompetitionRankings
            .AsNoTracking()
            .Include(r => r.Group)
            .Where(r => r.CompetitionId == request.CompetitionId)
            .OrderBy(r => r.RankOrder)
            .ToListAsync(cancellationToken);

        // Get solved exercises count and last submission time for each group
        var groupIds = rankings.Select(r => r.GroupId).ToList();
        var attempts = await _dbContext.GroupExerciseAttempts
            .AsNoTracking()
            .Where(a => a.CompetitionId == request.CompetitionId && groupIds.Contains(a.GroupId))
            .GroupBy(a => a.GroupId)
            .Select(g => new
            {
                GroupId = g.Key,
                SolvedCount = g.Where(a => a.Accepted).Select(a => a.ExerciseId).Distinct().Count(),
                LastSubmission = g.Max(a => (DateTime?)a.SubmissionTime)
            })
            .ToListAsync(cancellationToken);

        var attemptsDict = attempts.ToDictionary(a => a.GroupId);

        // Map to DTOs
        var rankingsDto = rankings.Select(r => new RankingEntryDto(
            r.GroupId,
            r.Group.Name,
            r.Points,
            r.Penalty,
            r.RankOrder,
            attemptsDict.ContainsKey(r.GroupId) ? attemptsDict[r.GroupId].SolvedCount : 0,
            attemptsDict.ContainsKey(r.GroupId) ? attemptsDict[r.GroupId].LastSubmission : null
        )).ToList();

        return new GetRankingResult(rankingsDto);
    }
}
