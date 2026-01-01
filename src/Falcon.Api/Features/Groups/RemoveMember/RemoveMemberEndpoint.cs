using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.RemoveMember;

/// <summary>
/// Endpoint for removing a member from a group.
/// </summary>
public class RemoveMemberEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/Group/{groupId}/member/{userId}", [Authorize] async (IMediator mediator, Guid groupId, string userId) =>
        {
            var command = new RemoveMemberCommand(groupId, userId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("RemoveMember")
        .WithTags("Groups")
        .Produces<RemoveMemberResult>();
    }
}
