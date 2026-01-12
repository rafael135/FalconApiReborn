using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Exercises.RemoveTestCase;

/// <summary>
/// Endpoint to remove a test case from an exercise.
/// </summary>
/// <remarks>
/// Example curl usage:
/// <code>
/// curl -X DELETE "https://localhost:5001/api/Exercise/{exerciseId}/testcase/{testCaseId}"
/// </code>
/// </remarks>
public class RemoveTestCaseEndpoint : IEndpoint
{
    /// <inheritdoc />
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
        .WithSummary("Remove a test case.")
        .WithDescription("Removes a test case from the specified exercise. Requires Teacher or Admin role.")
        .Produces<RemoveTestCaseResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
} 
