using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
        .WithSummary("Reject a group invitation.")
        .WithDescription("Rejects an outstanding invitation to join a group. Requires authentication.")
        .Produces<RejectInviteResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}
