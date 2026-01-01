using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.RemoveMember;

/// <summary>
/// Endpoint for removing a member from a group.
/// </summary>
[ApiController]
[Route("api/Group")]
public class RemoveMemberEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public RemoveMemberEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Removes a member from the group. Only the group leader can perform this action.
    /// </summary>
    /// <param name="groupId">The unique identifier of the group.</param>
    /// <param name="userId">The unique identifier of the user to remove.</param>
    /// <returns>Success status message.</returns>
    /// <response code="200">Member removed successfully.</response>
    /// <response code="400">Invalid request or user is the leader.</response>
    /// <response code="401">User is not authenticated or not the group leader.</response>
    /// <response code="404">Group or user not found.</response>
    [HttpDelete("{groupId}/member/{userId}")]
    [Authorize]
    [ProducesResponseType(typeof(RemoveMemberResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveMember([FromRoute] Guid groupId, [FromRoute] string userId)
    {
        var command = new RemoveMemberCommand(groupId, userId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
