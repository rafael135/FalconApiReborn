using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.AcceptInvite;

/// <summary>
/// Endpoint for accepting a group invitation.
/// </summary>
[ApiController]
[Route("api/Group")]
public class AcceptInviteEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public AcceptInviteEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Accepts a group invitation. The authenticated user must be the invite recipient.
    /// </summary>
    /// <param name="inviteId">The unique identifier of the invitation.</param>
    /// <returns>The group information after accepting.</returns>
    /// <response code="200">Invite accepted successfully.</response>
    /// <response code="400">Invalid request or user already in a group.</response>
    /// <response code="401">User is not authenticated or invite is not for them.</response>
    /// <response code="404">Invite not found.</response>
    [HttpPost("invite/{inviteId}/accept")]
    [Authorize]
    [ProducesResponseType(typeof(AcceptInviteResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptInvite([FromRoute] Guid inviteId)
    {
        var command = new AcceptInviteCommand(inviteId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
