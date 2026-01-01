using Falcon.Api.Features.Competitions.Shared;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.StartCompetition;

/// <summary>
/// Handler for starting a competition.
/// </summary>
public class StartCompetitionHandler : IRequestHandler<StartCompetitionCommand, StartCompetitionResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<StartCompetitionHandler> _logger;

    public StartCompetitionHandler(
        FalconDbContext dbContext,
        ILogger<StartCompetitionHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<StartCompetitionResult> Handle(StartCompetitionCommand request, CancellationToken cancellationToken)
    {
        var competition = await _dbContext.Competitions
            .FirstOrDefaultAsync(c => c.Id == request.CompetitionId, cancellationToken);

        if (competition == null)
        {
            throw new NotFoundException("Competition", request.CompetitionId);
        }

        // Verify competition is in a valid state to start
        if (competition.Status != CompetitionStatus.OpenInscriptions && 
            competition.Status != CompetitionStatus.ClosedInscriptions &&
            competition.Status != CompetitionStatus.Pending)
        {
            var errors = new Dictionary<string, string>
            {
                { "status", "Competição não está em um estado válido para iniciar" }
            };
            throw new FormException(errors);
        }

        // Call domain method to start
        competition.Start();

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Competition {CompetitionId} started", request.CompetitionId);

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

        return new StartCompetitionResult(competitionDto);
    }
}
