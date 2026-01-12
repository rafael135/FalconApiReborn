using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.AddTestCase;

/// <summary>
/// Endpoint for adding a test case to an exercise.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X POST "https://localhost:5001/api/Exercise/{exerciseId}/testcase" \
///  -H "Content-Type: application/json" \
///  -d '{ "exerciseId": "00000000-0000-0000-0000-000000000000", "inputContent": "1 2", "expectedOutput": "3" }'
/// </code>
/// </remarks>
public class AddTestCaseEndpoint : IEndpoint
{
    /// <inheritdoc />
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
        .WithSummary("Add a test case to an exercise.")
        .WithDescription("Adds a test case to the specified exercise. Requires Teacher or Admin role; route id must match command.ExerciseId.")
        .Produces<AddTestCaseResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
} 
