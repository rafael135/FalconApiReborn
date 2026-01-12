using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Competitions.UnregisterGroup;

/// <summary>
/// Endpoint for unregistering the current user's group from a competition. Requires authentication and group leadership.
/// </summary>
/// <remarks>
/// Example:
/// <code>
/// curl -X DELETE "https://localhost:5001/api/Competition/00000000-0000-0000-0000-000000000000/registration" \
///   -H "Authorization: Bearer {token}"
/// </code>
/// </remarks>
public class UnregisterGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/Competition/{id}/registration", [Authorize] async (IMediator mediator, Guid id) =>
        {
            var command = new UnregisterGroupCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UnregisterGroup")
        .WithTags("Competitions")
        .WithSummary("Unregister the current user's group from a competition.")
        .WithDescription("Unregisters the authenticated user's group from the specified competition. Requires group leadership.")
        .Produces<UnregisterGroupResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
