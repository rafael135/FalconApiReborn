using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Competitions.BlockGroup;

/// <summary>
/// Endpoint for blocking a group in a competition.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X POST "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000/block/00000000-0000-0000-0000-000000000000" \
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class BlockGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Competition/{competitionId}/block/{groupId}", [Authorize(Roles = "Teacher,Admin")] async (IMediator mediator, Guid competitionId, Guid groupId) =>
        {
            var command = new BlockGroupCommand(competitionId, groupId);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("BlockGroup")
        .WithTags("Competitions")
        .Produces<BlockGroupResult>();
    }
}
