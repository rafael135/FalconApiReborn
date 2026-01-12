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
        .WithSummary("Create a new group.")
        .WithDescription("Creates a new group. The creator is set as group leader.")
        .Produces<CreateGroupResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
