using Falcon.Core.Domain.Competitions;
using Falcon.Infrastructure.Database;
using Quartz;

namespace Falcon.Api.Features.Competitions.UpdateState;

/// <summary>
/// Job for processing and updating the state of a competition.
/// </summary>
public class ProcessCompetitionStateJob : IJob
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<ProcessCompetitionStateJob> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessCompetitionStateJob"/> class.
    /// </summary>
    /// <param name="dbContext">The database context for accessing competition data.</param>
    /// <param name="logger">The logger instance for logging information and errors.</param>
    public ProcessCompetitionStateJob(
        FalconDbContext dbContext,
        ILogger<ProcessCompetitionStateJob> logger
    )
    {
        this._dbContext = dbContext;
        this._logger = logger;
    }

    /// <summary>
    /// Executes the job to process and update the competition state.
    /// </summary>
    /// <param name="context">The context in which the job is executed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Execute(IJobExecutionContext context)
    {
        if (
            !Guid.TryParse(
                context.JobDetail.JobDataMap.GetString("CompetitionId"),
                out var competitionId
            )
        )
        {
            this._logger.LogError("Invalid CompetitionId in job data map.");
            return;
        }

        Competition? competition = await this._dbContext.Competitions.FindAsync(competitionId);
        if (competition == null)
        {
            this._logger.LogError($"Competition with ID {competitionId} not found.");
            return;
        }

        this._logger.LogInformation(
            $"Processing competition state for Competition ID: {competitionId}"
        );

        CompetitionStatus oldStatus = competition.Status;
        competition.UpdateStatusBasedOnTime(DateTime.UtcNow);

        if (competition.Status != oldStatus)
        {
            await this._dbContext.SaveChangesAsync();
            this._logger.LogInformation(
                $"Competition ID: {competitionId} status changed from {oldStatus} to {competition.Status}"
            );
        }
    }
}
