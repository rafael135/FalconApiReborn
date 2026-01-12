using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Groups.GetGroup;

/// <summary>
/// Endpoint for retrieving a group by ID.
/// </summary>
public class GetGroupEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/Group/{id}", async (IMediator mediator, Guid id) =>
        {
            var query = new GetGroupQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetGroup")
        .WithTags("Groups")
        .WithSummary("Get group details.")
        .WithDescription("Returns detailed information about a group, including members and invites.")
        .Produces<GetGroupResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
