using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.AddTestCase;

[ApiController]
[Route("api/Exercise")]
public class AddTestCaseEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public AddTestCaseEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpPost("{id}/testcase")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(AddTestCaseResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddTestCase([FromRoute] Guid id, [FromBody] AddTestCaseCommand command)
    {
        if (command.ExerciseId != id)
            return BadRequest(new { error = "Route ID does not match command ExerciseId" });

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
