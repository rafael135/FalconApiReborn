using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.CreateGroup;

/// <summary>
/// Endpoint for creating a new group.
/// </summary>
public class CreateGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Group", [Authorize] async (IMediator mediator, [FromBody] CreateGroupCommand command) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateGroup")
        .WithTags("Groups")
        .Produces<CreateGroupResult>();
    }
}
