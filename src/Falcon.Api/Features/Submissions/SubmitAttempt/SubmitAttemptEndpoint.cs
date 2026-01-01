using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

[ApiController]
[Route("api/Submission")]
public class SubmitAttemptEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public SubmitAttemptEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(SubmitAttemptResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitAttempt([FromBody] SubmitAttemptCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
