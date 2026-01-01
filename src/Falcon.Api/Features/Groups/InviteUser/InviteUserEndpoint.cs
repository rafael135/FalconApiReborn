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
        app.MapPost("api/Group/{groupId}/invite", [Authorize] async (IMediator mediator, Guid groupId, [FromBody] InviteUserCommand command) =>
        {
            if (groupId != command.GroupId) return Results.BadRequest("O ID da rota não corresponde ao ID do comando");
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("InviteUser")
        .WithTags("Groups")
        .Produces<InviteUserResult>();
    }
}
