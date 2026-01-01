using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.BlockGroup;

/// <summary>
/// Endpoint for blocking a group in a competition.
/// </summary>
[ApiController]
[Route("api/Competition")]
public class BlockGroupEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public BlockGroupEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Blocks a group from submitting in a competition. Only Teachers and Admins can perform this action.
    /// </summary>
    /// <param name="competitionId">The competition ID.</param>
    /// <param name="groupId">The group ID to block.</param>
    /// <returns>Success status message.</returns>
    /// <response code="200">Group blocked successfully.</response>
    /// <response code="400">Group is not registered in the competition.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have Teacher or Admin role.</response>
    [HttpPost("{competitionId}/block/{groupId}")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(BlockGroupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> BlockGroup([FromRoute] Guid competitionId, [FromRoute] Guid groupId)
    {
        var command = new BlockGroupCommand(competitionId, groupId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
