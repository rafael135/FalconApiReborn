using Falcon.Api.Features.Submissions.Shared;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.GetAttempts;

public class GetAttemptsHandler : IRequestHandler<GetAttemptsQuery, GetAttemptsResult>
{
    private readonly FalconDbContext _dbContext;

    public GetAttemptsHandler(FalconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetAttemptsResult> Handle(GetAttemptsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.GroupExerciseAttempts
            .AsNoTracking()
            .Include(a => a.Exercise)
            .Include(a => a.Group)
            .Where(a => a.CompetitionId == request.CompetitionId);

        var totalCount = await query.CountAsync(cancellationToken);

        var attempts = await query
            .OrderByDescending(a => a.SubmissionTime)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(a => new AttemptDto(
                a.Id, a.ExerciseId, a.Exercise.Title, a.GroupId, a.Group.Name,
                a.SubmissionTime, a.Language, a.Accepted, a.JudgeResponse, a.Time
            ))
            .ToListAsync(cancellationToken);

        return new GetAttemptsResult(attempts, totalCount);
    }
}
