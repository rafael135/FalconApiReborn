using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.RemoveTestCase;

public class RemoveTestCaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/Exercise/{exerciseId}/testcase/{testCaseId}", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid exerciseId, Guid testCaseId) =>
        {
            var command = new RemoveTestCaseCommand(exerciseId, testCaseId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("RemoveTestCase")
        .WithTags("Exercises")
        .Produces<RemoveTestCaseResult>();
    }
}
