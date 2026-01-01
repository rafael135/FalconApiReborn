using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.UpdateGroup;

/// <summary>
/// Endpoint for updating a group's name.
/// </summary>
[ApiController]
[Route("api/Group")]
public class UpdateGroupEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateGroupEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Updates a group's name. Only the group leader can perform this action.
    /// </summary>
    /// <param name="id">The unique identifier of the group.</param>
    /// <param name="command">The update command containing the new name.</param>
    /// <returns>The updated group information.</returns>
    /// <response code="200">Group updated successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">User is not authenticated or not the group leader.</response>
    /// <response code="404">Group not found.</response>
    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(typeof(UpdateGroupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateGroup([FromRoute] Guid id, [FromBody] UpdateGroupCommand command)
    {
        // Ensure the ID from route matches the command
        if (id != command.GroupId)
        {
            return BadRequest("O ID da rota não corresponde ao ID do comando");
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
