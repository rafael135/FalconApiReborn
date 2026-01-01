using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.GetExercises;

/// <summary>
/// Endpoint for getting exercises with optional filtering.
/// </summary>
[ApiController]
[Route("api/Exercise")]
public class GetExercisesEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetExercisesEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a paginated list of exercises with optional exercise type filter.
    /// </summary>
    /// <param name="exerciseTypeId">Optional exercise type filter.</param>
    /// <param name="skip">Number of items to skip (default: 0).</param>
    /// <param name="take">Number of items to take (default: 10).</param>
    /// <returns>Paginated list of exercises.</returns>
    /// <response code="200">Exercises retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetExercisesResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExercises(
        [FromQuery] int? exerciseTypeId = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10)
    {
        var query = new GetExercisesQuery(exerciseTypeId, skip, take);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
