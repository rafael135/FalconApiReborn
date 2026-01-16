using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

/// <summary>
/// Endpoint for submitting an exercise attempt.
/// </summary>
/// <remarks>
/// Submissions are processed asynchronously by the Worker and the final result is sent via SignalR (SubmitExerciseResult consumer).
/// Example request: { "competitionId": "...", "exerciseId": "...", "code": "print(1+1)", "language": "Python" }
/// </remarks>
public class SubmitAttemptEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/Submission",
                [Authorize]
                async (IMediator mediator, [FromBody] SubmitAttemptCommand command) =>
                {
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
            )
            .WithName("SubmitAttempt")
            .WithTags("Submissions")
            .WithSummary("Submit an exercise attempt.")
            .WithDescription(
                "Submit code for an exercise; processing is asynchronous and results are delivered via SignalR."
            )
            .Produces<SubmitAttemptResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
