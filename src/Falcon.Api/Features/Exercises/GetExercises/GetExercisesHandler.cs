using Falcon.Api.Features.Exercises.Shared;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Exercises.GetExercises;

/// <summary>
/// Handler for getting exercises with optional filtering and pagination.
/// </summary>
public class GetExercisesHandler : IRequestHandler<GetExercisesQuery, GetExercisesResult>
{
    private readonly FalconDbContext _dbContext;

    public GetExercisesHandler(FalconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetExercisesResult> Handle(GetExercisesQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Core.Domain.Exercises.Exercise> query = _dbContext.Exercises
            .AsNoTracking()
            .Include(e => e.ExerciseType);

        // Filter by ExerciseType if provided
        if (request.ExerciseTypeId.HasValue)
        {
            query = query.Where(e => e.ExerciseTypeId == request.ExerciseTypeId.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and ordering
        var exercises = await query
            .OrderBy(e => e.Title)
            .Skip(request.Skip)
            .Take(request.Take)
            .Select(e => new ExerciseSummaryDto(
                e.Id,
                e.Title,
                e.EstimatedTime,
                e.ExerciseType.Label
            ))
            .ToListAsync(cancellationToken);

        return new GetExercisesResult(
            exercises,
            totalCount,
            request.Skip,
            request.Take
        );
    }
}
