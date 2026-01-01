using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.CreateGroup;

/// <summary>
/// Endpoint for creating a new group.
/// </summary>
[ApiController]
[Route("api/Group")]
public class CreateGroupEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateGroupEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new group with the authenticated user as the leader.
    /// </summary>
    /// <param name="command">The group creation command containing the group name.</param>
    /// <returns>The created group information.</returns>
    /// <response code="200">Group created successfully.</response>
    /// <response code="400">Invalid request or user already in a group.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CreateGroupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
