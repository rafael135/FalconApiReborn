using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.GetGroupAttempts;

[ApiController]
[Route("api/Submission")]
public class GetGroupAttemptsEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetGroupAttemptsEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpGet("group")]
    [Authorize]
    [ProducesResponseType(typeof(GetGroupAttemptsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroupAttempts([FromQuery] Guid? competitionId, [FromQuery] Guid? exerciseId)
    {
        var query = new GetGroupAttemptsQuery(competitionId, exerciseId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
