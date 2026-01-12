using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

/// <summary>
/// Endpoint for submitting an exercise attempt.
/// </summary>
/// <remarks>
/// Submissões são processadas de forma assíncrona pelo Worker; o resultado final é enviado via SignalR (SubmitExerciseResult consumer).
/// Exemplo de request: { "competitionId": "...", "exerciseId": "...", "code": "print(1+1)", "language": "Python" }
/// </remarks>
public class SubmitAttemptEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Submission", [Authorize] async (
            IMediator mediator,
            [FromBody] SubmitAttemptCommand command) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("SubmitAttempt")
        .WithTags("Submissions")
        .WithSummary("Submit an exercise attempt.")
        .WithDescription("Submit code for an exercise; processing is asynchronous and results are delivered via SignalR.")
        .Produces<SubmitAttemptResult>();
    }
}
