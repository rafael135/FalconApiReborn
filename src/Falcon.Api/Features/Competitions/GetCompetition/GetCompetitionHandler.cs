using Falcon.Api.Features.Competitions.Shared;
using Falcon.Api.Features.Exercises.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.GetCompetition;

/// <summary>
/// Handler for getting detailed competition information.
/// </summary>
public class GetCompetitionHandler : IRequestHandler<GetCompetitionQuery, GetCompetitionResult>
{
    private readonly FalconDbContext _dbContext;

    public GetCompetitionHandler(FalconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Handles the <see cref="GetCompetitionQuery"/> and returns detailed competition information.
    /// </summary>
    /// <param name="request">Query containing the competition id to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="GetCompetitionResult"/> with full competition details.</returns>
    /// <exception cref="NotFoundException">Thrown when the competition is not found.</exception>
    public async Task<GetCompetitionResult> Handle(GetCompetitionQuery request, CancellationToken cancellationToken)
    {
        var competition = await _dbContext.Competitions
            .AsNoTracking()
            .Include(c => c.ExercisesInCompetition)
                .ThenInclude(ec => ec.Exercise)
                    .ThenInclude(e => e.ExerciseType)
            .Include(c => c.Rankings)
                .ThenInclude(r => r.Group)
            .FirstOrDefaultAsync(c => c.Id == request.CompetitionId, cancellationToken);

        if (competition == null)
        {
            throw new NotFoundException("Competition", request.CompetitionId);
        }

        // Map exercises
        var exercisesDto = competition.ExercisesInCompetition
            .Select(ec => new ExerciseSummaryDto(
                ec.Exercise.Id,
                ec.Exercise.Title,
                ec.Exercise.EstimatedTime,
                ec.Exercise.ExerciseType.Label
            ))
            .ToList();

        // Map rankings
        var rankingsDto = competition.Rankings
            .OrderBy(r => r.RankOrder)
            .Select(r => new RankingEntryDto(
                r.GroupId,
                r.Group.Name,
                r.Points,
                r.Penalty,
                r.RankOrder,
                0, // SolvedExercises - will be calculated from attempts
                null // LastSubmissionTime - will be calculated from attempts
            ))
            .ToList();

        var competitionDetailDto = new CompetitionDetailDto(
            competition.Id,
            competition.Name,
            competition.Description,
            competition.Status,
            competition.StartInscriptions,
            competition.EndInscriptions,
            competition.StartTime,
            competition.EndTime,
            competition.MaxMembers,
            competition.MaxExercises,
            competition.MaxSubmissionSize,
            competition.Duration,
            competition.SubmissionPenalty,
            exercisesDto,
            rankingsDto
        );

        return new GetCompetitionResult(competitionDetailDto);
    }
}
