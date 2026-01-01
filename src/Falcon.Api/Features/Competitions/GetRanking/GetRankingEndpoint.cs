using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.GetRanking;

/// <summary>
/// Endpoint for getting competition ranking.
/// </summary>
[ApiController]
[Route("api/Competition")]
public class GetRankingEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public GetRankingEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets the ranking for a specific competition.
    /// </summary>
    /// <param name="id">The competition ID.</param>
    /// <returns>Competition ranking ordered by position.</returns>
    /// <response code="200">Ranking retrieved successfully.</response>
    /// <response code="404">Competition not found.</response>
    [HttpGet("{id}/ranking")]
    [ProducesResponseType(typeof(GetRankingResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRanking([FromRoute] Guid id)
    {
        var query = new GetRankingQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
