using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.StartCompetition;

/// <summary>
/// Endpoint for starting a competition.
/// </summary>
[ApiController]
[Route("api/Competition")]
public class StartCompetitionEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public StartCompetitionEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Starts a competition. Only Teachers and Admins can perform this action.
    /// </summary>
    /// <param name="id">The competition ID.</param>
    /// <returns>The updated competition.</returns>
    /// <response code="200">Competition started successfully.</response>
    /// <response code="400">Competition is not in a valid state to start.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have Teacher or Admin role.</response>
    /// <response code="404">Competition not found.</response>
    [HttpPost("{id}/start")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(StartCompetitionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartCompetition([FromRoute] Guid id)
    {
        var command = new StartCompetitionCommand(id);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
