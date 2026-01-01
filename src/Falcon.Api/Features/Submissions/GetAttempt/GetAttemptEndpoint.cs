using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.GetAttempt;

[ApiController]
[Route("api/Submission")]
public class GetAttemptEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetAttemptEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(GetAttemptResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttempt([FromRoute] Guid id)
    {
        var query = new GetAttemptQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
