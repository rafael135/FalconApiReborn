using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.LeaveGroup;

/// <summary>
/// Endpoint for a user to leave their current group.
/// </summary>
[ApiController]
[Route("api/Group")]
public class LeaveGroupEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public LeaveGroupEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Allows the authenticated user to leave their current group.
    /// The group leader cannot leave without transferring leadership first.
    /// </summary>
    /// <returns>Success status message.</returns>
    /// <response code="200">User left the group successfully.</response>
    /// <response code="400">User is not in a group or is the group leader.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost("leave")]
    [Authorize]
    [ProducesResponseType(typeof(LeaveGroupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LeaveGroup()
    {
        var command = new LeaveGroupCommand();
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
