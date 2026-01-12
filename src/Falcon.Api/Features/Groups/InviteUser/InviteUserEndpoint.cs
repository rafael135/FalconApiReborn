using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Groups.InviteUser;

/// <summary>
/// Endpoint for inviting a user to a group.
/// </summary>
public class InviteUserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Group/invite", [Authorize] async (IMediator mediator, [FromBody] InviteUserCommand command) =>
        {
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("InviteUser")
        .WithTags("Groups")
        .WithSummary("Invite a user to the group.")
        .WithDescription("Sends an invitation to the specified user to join the authenticated user's group. Requires group leadership.")
        .Produces<InviteUserResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden);

    }
}
