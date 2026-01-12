using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Groups.AcceptInvite;

/// <summary>
/// Endpoint for accepting a group invitation.
/// </summary>
public class AcceptInviteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Group/invite/{inviteId}/accept", [Authorize] async (IMediator mediator, Guid inviteId) =>
        {
            var command = new AcceptInviteCommand(inviteId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("AcceptInvite")
        .WithTags("Groups")
        .WithSummary("Accept a group invitation.")
        .WithDescription("Accepts an invitation to join a group. Requires authentication.")
        .Produces<AcceptInviteResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound);
    }
}
