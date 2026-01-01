using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.RemoveTestCase;

[ApiController]
[Route("api/Exercise")]
public class RemoveTestCaseEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public RemoveTestCaseEndpoint(IMediator mediator) => _mediator = mediator;

    [HttpDelete("{exerciseId}/testcase/{testCaseId}")]
    [Authorize(Roles = "Teacher,Admin")]
    [ProducesResponseType(typeof(RemoveTestCaseResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveTestCase([FromRoute] Guid exerciseId, [FromRoute] Guid testCaseId)
    {
        var command = new RemoveTestCaseCommand(exerciseId, testCaseId);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
