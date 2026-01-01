using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.GetExercise;

/// <summary>
/// Endpoint for getting an exercise by ID.
/// </summary>
[ApiController]
[Route("api/Exercise")]
public class GetExerciseEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetExerciseEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets detailed information about an exercise. Test cases are only visible to Teachers and Admins.
    /// </summary>
    /// <param name="id">The exercise ID.</param>
    /// <returns>Detailed exercise information.</returns>
    /// <response code="200">Exercise retrieved successfully.</response>
    /// <response code="404">Exercise not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetExerciseResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExercise([FromRoute] Guid id)
    {
        var query = new GetExerciseQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
