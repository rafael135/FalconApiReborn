using Falcon.Api.Features.Competitions.UpdateState;
using Falcon.Core.Domain.Competitions;
using Quartz;
using Quartz.Impl.Matchers;

namespace Falcon.Api.Features.Competitions.Services;

/// <summary>
/// Service responsible for scheduling competition state change jobs.
/// </summary>
public class CompetitionScheduler
{
    private readonly ISchedulerFactory _schedulerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompetitionScheduler"/> class.
    /// </summary>
    /// <param name="schedulerFactory">The factory used to create schedulers.</param>
    public CompetitionScheduler(ISchedulerFactory schedulerFactory)
    {
        this._schedulerFactory = schedulerFactory;
    }

    /// <summary>
    /// Schedules jobs to process competition state changes at key dates.
    /// </summary>
    /// <param name="competition">The competition for which to schedule state change jobs.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ScheduleCompetitionChanges(Competition competition)
    {
        IScheduler scheduler = await this._schedulerFactory.GetScheduler();
        string groupId = $"Competition-{competition.Id}";

        await DeleteSchedule(competition.Id);

        DateTime[] keyDates = new[]
        {
            competition.StartInscriptions!,
            competition.EndInscriptions!,
            competition.StartTime,
            competition.EndTime!.Value,
        };

        foreach (var date in keyDates)
        {
            DateTime dateUtc = DateTime.SpecifyKind(date, DateTimeKind.Utc);
            DateTimeOffset runAt = dateUtc;

            if (runAt > DateTimeOffset.UtcNow)
            {
                TriggerKey triggerKey = new TriggerKey($"{Guid.NewGuid()}", groupId);

                IJobDetail jobDetail = JobBuilder
                    .Create<ProcessCompetitionStateJob>()
                    .WithIdentity(Guid.NewGuid().ToString(), groupId)
                    .UsingJobData("CompetitionId", competition.Id.ToString())
                    .Build();

                ITrigger trigger = TriggerBuilder
                    .Create()
                    .WithIdentity(triggerKey)
                    .StartAt(runAt)
                    .Build();

                await scheduler.ScheduleJob(jobDetail, trigger);
            }
        }
    }

    /// <summary>
    /// Deletes all scheduled jobs for a specific competition.
    /// </summary>
    /// <param name="competitionId">The unique identifier of the competition.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteSchedule(Guid competitionId)
    {
        IScheduler scheduler = await this._schedulerFactory.GetScheduler();
        string groupId = $"Competition-{competitionId}";

        GroupMatcher<JobKey> groupMatcher = Quartz.Impl.Matchers.GroupMatcher<JobKey>.GroupEquals(
            groupId
        );
        var jobKeys = await scheduler.GetJobKeys(groupMatcher);

        await scheduler.DeleteJobs(jobKeys);
    }
}
