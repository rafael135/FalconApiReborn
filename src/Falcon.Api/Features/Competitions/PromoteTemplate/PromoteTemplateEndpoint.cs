using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.PromoteTemplate;

/// <summary>
/// Endpoint for promoting a competition template to an active competition.
/// </summary>
[ApiController]
[Route("api/Competition")]
public class PromoteTemplateEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public PromoteTemplateEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Promotes a template to an active competition. Only Teachers and Admins can perform this action.
    /// </summary>
    /// <param name="id">The template competition ID.</param>
    /// <param name="command">The promotion parameters.</param>
    /// <returns>The promoted competition.</returns>
    /// <response code="200">Template promoted successfully.</response>
    /// <response code="400">Invalid request data or competition is not a template.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have Teacher or Admin role.</response>
    /// <response code="404">Template not found.</response>
    [HttpPost("{id}/promote")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(PromoteTemplateResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PromoteTemplate(
        [FromRoute] Guid id, 
        [FromBody] PromoteTemplateCommand command)
    {
        // Ensure route ID matches command
        if (command.TemplateId != id)
        {
            return BadRequest(new { error = "Route ID does not match command TemplateId" });
        }

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
