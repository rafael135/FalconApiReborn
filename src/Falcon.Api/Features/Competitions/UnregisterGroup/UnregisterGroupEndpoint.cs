using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.UnregisterGroup;

/// <summary>
/// Endpoint for unregistering a group from a competition.
/// </summary>
[ApiController]
[Route("api/Competition")]
public class UnregisterGroupEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public UnregisterGroupEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Unregisters the authenticated user's group from a competition. Only the group leader can perform this action.
    /// </summary>
    /// <param name="id">The competition ID.</param>
    /// <returns>Success status message.</returns>
    /// <response code="200">Group unregistered successfully.</response>
    /// <response code="400">Competition already started or group not registered.</response>
    /// <response code="401">User is not authenticated or not the group leader.</response>
    /// <response code="404">Competition not found.</response>
    [HttpDelete("{id}/registration")]
    [Authorize]
    [ProducesResponseType(typeof(UnregisterGroupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnregisterGroup([FromRoute] Guid id)
    {
        var command = new UnregisterGroupCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
