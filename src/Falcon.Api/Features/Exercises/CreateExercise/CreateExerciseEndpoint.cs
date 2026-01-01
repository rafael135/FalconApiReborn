using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.CreateExercise;

/// <summary>
/// Endpoint for creating a new exercise.
/// </summary>
[ApiController]
[Route("api/Exercise")]
public class CreateExerciseEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateExerciseEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new exercise. Only Teachers and Admins can create exercises.
    /// </summary>
    /// <param name="command">The exercise creation command.</param>
    /// <returns>The created exercise.</returns>
    /// <response code="200">Exercise created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have Teacher or Admin role.</response>
    [HttpPost]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(CreateExerciseResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateExercise([FromBody] CreateExerciseCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
