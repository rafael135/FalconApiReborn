using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Groups.LeaveGroup;

/// <summary>
/// Endpoint for a user to leave their current group.
/// </summary>
public class LeaveGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Group/leave", [Authorize] async (IMediator mediator) =>
        {
            var command = new LeaveGroupCommand();
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("LeaveGroup")
        .WithTags("Groups")
        .Produces<LeaveGroupResult>();
    }
}
