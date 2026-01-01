using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.RemoveExercise;

[ApiController]
[Route("api/Competition")]
public class RemoveExerciseEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public RemoveExerciseEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpDelete("{competitionId}/exercise/{exerciseId}")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(RemoveExerciseResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveExercise([FromRoute] Guid competitionId, [FromRoute] Guid exerciseId)
    {
        var command = new RemoveExerciseCommand(competitionId, exerciseId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
