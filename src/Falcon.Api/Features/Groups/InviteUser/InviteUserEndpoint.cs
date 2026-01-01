using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        .Produces<InviteUserResult>();
    }
}
