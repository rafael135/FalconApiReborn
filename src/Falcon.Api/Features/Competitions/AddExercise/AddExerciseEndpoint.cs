using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.AddExercise;

[ApiController]
[Route("api/Competition")]
public class AddExerciseEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public AddExerciseEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpPost("{competitionId}/exercise/{exerciseId}")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(AddExerciseResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddExercise([FromRoute] Guid competitionId, [FromRoute] Guid exerciseId)
    {
        var command = new AddExerciseCommand(competitionId, exerciseId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
