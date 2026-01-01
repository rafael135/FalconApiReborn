using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.SubmitAttempt;

/// <summary>
/// Endpoint for submitting an exercise attempt.
/// </summary>
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
        .Produces<SubmitAttemptResult>();
    }
}
