using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Submissions.GetAttempt;

public class GetAttemptEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Submission/{id}", [Authorize] async (
            IMediator mediator,
            Guid id) =>
        {
            var query = new GetAttemptQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetAttempt")
        .WithTags("Submissions")
        .Produces<GetAttemptResult>();
    }
}
