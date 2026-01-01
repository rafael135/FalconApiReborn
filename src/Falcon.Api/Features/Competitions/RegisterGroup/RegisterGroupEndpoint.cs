using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.RegisterGroup;

/// <summary>
/// Endpoint for registering a group in a competition.
/// </summary>
[ApiController]
[Route("api/Competition")]
public class RegisterGroupEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public RegisterGroupEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers the authenticated user's group in a competition.
    /// </summary>
    /// <param name="id">The competition ID.</param>
    /// <returns>The registration information.</returns>
    /// <response code="200">Group registered successfully.</response>
    /// <response code="400">Invalid request or business rule violation.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Competition not found.</response>
    [HttpPost("{id}/register")]
    [Authorize]
    [ProducesResponseType(typeof(RegisterGroupResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RegisterGroup([FromRoute] Guid id)
    {
        var command = new RegisterGroupCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
