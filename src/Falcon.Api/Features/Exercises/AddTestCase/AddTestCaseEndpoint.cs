using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.AddTestCase;

public class AddTestCaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Exercise/{id}/testcase", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid id, [FromBody] AddTestCaseCommand command) =>
        {
            if (command.ExerciseId != id) return Results.BadRequest(new { error = "Route ID does not match command ExerciseId" });
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("AddTestCase")
        .WithTags("Exercises")
        .Produces<AddTestCaseResult>();
    }
}
