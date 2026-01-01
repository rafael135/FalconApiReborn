using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.GetAttempts;

/// <summary>
/// Endpoint for getting attempts of a competition (Teacher/Admin only).
/// </summary>
public class GetAttemptsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Competition/{id}/attempts", [Authorize(Roles = "Teacher,Admin")] async (
            IMediator mediator,
            Guid id,
            int skip = 0,
            int take = 10) =>
        {
            var query = new GetAttemptsQuery(id, skip, take);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetAllAttempts")
        .WithTags("Competitions")
        .Produces<GetAttemptsResult>();
    }
}
