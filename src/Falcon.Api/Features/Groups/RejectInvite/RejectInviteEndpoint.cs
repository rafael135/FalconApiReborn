using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.RejectInvite;

/// <summary>
/// Endpoint for rejecting a group invitation.
/// </summary>
public class RejectInviteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Group/invite/{inviteId}/reject", [Authorize] async (IMediator mediator, Guid inviteId) =>
        {
            var command = new RejectInviteCommand(inviteId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("RejectInvite")
        .WithTags("Groups")
        .Produces<RejectInviteResult>();
    }
}
