using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.CreateTemplate;

/// <summary>
/// Endpoint for creating a competition template.
/// </summary>
[ApiController]
[Route("api/Competition")]
public class CreateTemplateEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public CreateTemplateEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new competition template. Only Teachers and Admins can create templates.
    /// </summary>
    /// <param name="command">The template creation command.</param>
    /// <returns>The created competition template.</returns>
    /// <response code="200">Template created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="403">User does not have Teacher or Admin role.</response>
    [HttpPost("template")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(CreateTemplateResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateTemplate([FromBody] CreateTemplateCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
