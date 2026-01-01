using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.RejectInvite;

/// <summary>
/// Endpoint for rejecting a group invitation.
/// </summary>
[ApiController]
[Route("api/Group")]
public class RejectInviteEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public RejectInviteEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Rejects a group invitation. The authenticated user must be the invite recipient.
    /// </summary>
    /// <param name="inviteId">The unique identifier of the invitation.</param>
    /// <returns>Success status message.</returns>
    /// <response code="200">Invite rejected successfully.</response>
    /// <response code="401">User is not authenticated or invite is not for them.</response>
    /// <response code="404">Invite not found.</response>
    [HttpPost("invite/{inviteId}/reject")]
    [Authorize]
    [ProducesResponseType(typeof(RejectInviteResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectInvite([FromRoute] Guid inviteId)
    {
        var command = new RejectInviteCommand(inviteId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
