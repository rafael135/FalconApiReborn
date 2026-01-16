using Falcon.Api.Features.Competitions.Shared;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.FinishCompetition;

/// <summary>
/// Handler for finishing a competition.
/// </summary>
public class FinishCompetitionHandler
    : IRequestHandler<FinishCompetitionCommand, FinishCompetitionResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<FinishCompetitionHandler> _logger;

    public FinishCompetitionHandler(
        FalconDbContext dbContext,
        ILogger<FinishCompetitionHandler> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="FinishCompetitionCommand"/> and finishes the competition if it is ongoing.
    /// </summary>
    /// <param name="request">Command containing the competition id to finish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated competition information after finishing.</returns>
    /// <exception cref="NotFoundException">Thrown when the competition is not found.</exception>
    /// <exception cref="FormException">Thrown when the competition is not in an ongoing state.</exception>
    public async Task<FinishCompetitionResult> Handle(
        FinishCompetitionCommand request,
        CancellationToken cancellationToken
    )
    {
        var competition = await _dbContext.Competitions.FirstOrDefaultAsync(
            c => c.Id == request.CompetitionId,
            cancellationToken
        );

        if (competition == null)
        {
            throw new NotFoundException("Competition", request.CompetitionId);
        }

        // Verify competition is ongoing
        if (competition.Status != CompetitionStatus.Ongoing)
        {
            var errors = new Dictionary<string, string>
            {
                { "status", "Apenas competições em andamento podem ser finalizadas" },
            };
            throw new FormException(errors);
        }

        // Call domain method to finish
        competition.Finish();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Competition {CompetitionId} finished", request.CompetitionId);

        var competitionDto = new CompetitionDto(
            competition.Id,
            competition.Name,
            competition.Description,
            competition.Status,
            competition.StartInscriptions,
            competition.EndInscriptions,
            competition.StartTime,
            competition.EndTime,
            competition.MaxMembers,
            competition.MaxExercises
        );

        return new FinishCompetitionResult(competitionDto);
    }
}
