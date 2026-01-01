using MediatR;
using Microsoft.AspNetCore.Mvc;
using Falcon.Core.Domain.Competitions;

namespace Falcon.Api.Features.Competitions.GetCompetitions;

/// <summary>
/// Endpoint for getting competitions with optional filtering.
/// </summary>
[ApiController]
[Route("api/Competition")]
public class GetCompetitionsEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetCompetitionsEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a paginated list of competitions with optional status filter.
    /// </summary>
    /// <param name="status">Optional competition status filter.</param>
    /// <param name="skip">Number of items to skip (default: 0).</param>
    /// <param name="take">Number of items to take (default: 10).</param>
    /// <returns>Paginated list of competitions.</returns>
    /// <response code="200">Competitions retrieved successfully.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetCompetitionsResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompetitions(
        [FromQuery] CompetitionStatus? status = null,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10)
    {
        var query = new GetCompetitionsQuery(status, skip, take);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
