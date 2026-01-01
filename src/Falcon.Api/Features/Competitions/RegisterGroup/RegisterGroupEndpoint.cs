using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.RegisterGroup;

/// <summary>
/// Endpoint for registering a group in a competition.
/// </summary>
public class RegisterGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Competition/{id}/register", [Authorize] async (IMediator mediator, Guid id) =>
        {
            var command = new RegisterGroupCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("RegisterGroup")
        .WithTags("Competitions")
        .Produces<RegisterGroupResult>();
    }
}
