using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.GetGroup;

/// <summary>
/// Endpoint for retrieving a group by ID.
/// </summary>
[ApiController]
[Route("api/Group")]
public class GetGroupEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetGroupEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves detailed information about a specific group.
    /// </summary>
    /// <param name="id">The unique identifier of the group.</param>
    /// <returns>The group information including members and pending invites.</returns>
    /// <response code="200">Group retrieved successfully.</response>
    /// <response code="404">Group not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetGroupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGroup([FromRoute] Guid id)
    {
        var query = new GetGroupQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
