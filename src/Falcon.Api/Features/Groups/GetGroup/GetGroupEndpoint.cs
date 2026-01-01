using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        .Produces<GetGroupResult>();
    }
}
