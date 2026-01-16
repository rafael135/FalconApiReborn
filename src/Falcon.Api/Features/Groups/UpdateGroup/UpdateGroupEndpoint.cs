using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.UpdateGroup;

/// <summary>
/// Endpoint for updating a group's name.
/// </summary>
public class UpdateGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "api/Group/{id}",
                [Authorize]
                async (IMediator mediator, Guid id, [FromBody] UpdateGroupCommand command) =>
                {
                    if (id != command.GroupId)
                        return Results.BadRequest("Route id does not match command.GroupId");
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
            )
            .WithName("UpdateGroup")
            .WithTags("Groups")
            .WithSummary("Update a group.")
            .WithDescription(
                "Updates group information such as the group name. Requires authentication and appropriate permissions."
            )
            .Produces<UpdateGroupResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
