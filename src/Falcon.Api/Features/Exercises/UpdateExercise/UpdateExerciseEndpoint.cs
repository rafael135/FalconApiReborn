using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

[ApiController]
[Route("api/Exercise")]
public class UpdateExerciseEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public UpdateExerciseEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpPut("{id}")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(UpdateExerciseResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateExercise([FromRoute] Guid id, [FromBody] UpdateExerciseCommand command)
    {
        if (command.ExerciseId != id)
            return BadRequest(new { error = "Route ID does not match command ExerciseId" });

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
