using Falcon.Core.Domain.Competitions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.UpdateState;

/// <summary>
/// Handler for updating the state of competitions based on their timelines.
/// </summary>
public class UpdateCompetitionStateHandler : IRequestHandler<UpdateCompetitionStateCommand>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<UpdateCompetitionStateHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCompetitionStateHandler"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing competition data.</param>
    /// <param name="logger">The logger instance for logging information and errors.</param>
    public UpdateCompetitionStateHandler(
        FalconDbContext dbContext,
        ILogger<UpdateCompetitionStateHandler> logger
    )
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Handles the update competition state command.
    /// </summary>
    /// <param name="request">The command containing information to update competition states.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Handle(
        UpdateCompetitionStateCommand request,
        CancellationToken cancellationToken
    )
    {
        var activeCompetitions = await _dbContext
            .Competitions.Where(c =>
                c.Status != CompetitionStatus.Finished
                && c.Status != CompetitionStatus.ModelTemplate
            )
            .ToListAsync(cancellationToken);

        if (!activeCompetitions.Any())
        {
            return;
        }

        DateTime now = DateTime.UtcNow;
        bool anyChanged = false;

        foreach (var competition in activeCompetitions)
        {
            CompetitionStatus oldStatus = competition.Status;
            competition.UpdateStatusBasedOnTime(now);

            if (competition.Status != oldStatus)
            {
                anyChanged = true;
                _logger.LogInformation(
                    "Competition {CompetitionId} status changed from {OldStatus} to {NewStatus}",
                    competition.Id,
                    oldStatus,
                    competition.Status
                );
            }
        }

        if (anyChanged)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
