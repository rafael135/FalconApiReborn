using MediatR;
using Quartz;

namespace Falcon.Api.Features.Competitions.UpdateState;

/// <summary>
/// Job for updating the state of competitions.
/// </summary>
[DisallowConcurrentExecution]
public class UpdateCompetitionStateJob : IJob
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateCompetitionStateJob"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands.</param>
    public UpdateCompetitionStateJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Executes the job to update competition states.
    /// </summary>
    /// <param name="context">The context in which the job is executed.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task Execute(IJobExecutionContext context)
    {
        await _mediator.Send(new UpdateCompetitionStateCommand());
    }
}
