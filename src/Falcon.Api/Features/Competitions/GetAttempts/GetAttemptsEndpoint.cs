using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.GetAttempts;

[ApiController]
[Route("api/Competition")]
public class GetAttemptsEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetAttemptsEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}/attempts")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(GetAttemptsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttempts([FromRoute] Guid id, [FromQuery] int skip = 0, [FromQuery] int take = 10)
    {
        var query = new GetAttemptsQuery(id, skip, take);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
