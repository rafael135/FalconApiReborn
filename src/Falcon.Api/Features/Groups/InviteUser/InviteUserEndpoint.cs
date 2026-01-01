using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.InviteUser;

/// <summary>
/// Endpoint for inviting a user to a group.
/// </summary>
[ApiController]
[Route("api/Group")]
public class InviteUserEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public InviteUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Invites a user to join the group. Only the group leader can invite users.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="command">The invite command containing the user ID.</param>
    /// <returns>The created invite information.</returns>
    /// <response code="200">Invite created successfully.</response>
    /// <response code="400">Invalid request or user already in a group.</response>
    /// <response code="401">User is not authenticated or not the group leader.</response>
    /// <response code="404">Group or target user not found.</response>
    [HttpPost("{groupId}/invite")]
    [Authorize]
    [ProducesResponseType(typeof(InviteUserResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> InviteUser([FromRoute] Guid groupId, [FromBody] InviteUserCommand command)
    {
        // Ensure the ID from route matches the command
        if (groupId != command.GroupId)
        {
            return BadRequest("O ID da rota não corresponde ao ID do comando");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
