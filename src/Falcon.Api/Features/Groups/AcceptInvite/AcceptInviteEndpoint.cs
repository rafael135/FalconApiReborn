using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        .Produces<AcceptInviteResult>();
    }
}
