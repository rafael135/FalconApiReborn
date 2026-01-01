using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.GetCompetition;

/// <summary>
/// Endpoint for getting detailed competition information.
/// </summary>
[ApiController]
[Route("api/Competition")]
public class GetCompetitionEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetCompetitionEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets detailed information about a specific competition.
    /// </summary>
    /// <param name="id">The competition ID.</param>
    /// <returns>Detailed competition information.</returns>
    /// <response code="200">Competition retrieved successfully.</response>
    /// <response code="404">Competition not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetCompetitionResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCompetition([FromRoute] Guid id)
    {
        var query = new GetCompetitionQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
