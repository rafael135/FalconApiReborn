using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.UnregisterGroup;

/// <summary>
/// Endpoint for unregistering a group from a competition.
/// </summary>
public class UnregisterGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/Competition/{id}/registration", [Authorize] async (IMediator mediator, Guid id) =>
        {
            var command = new UnregisterGroupCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UnregisterGroup")
        .WithTags("Competitions")
        .Produces<UnregisterGroupResult>();
    }
}
