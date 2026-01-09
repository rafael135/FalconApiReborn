using Falcon.Api.Extensions;
using Falcon.Api.Features.Users.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Users.GetCompetitionHistory;

/// <summary>
/// Endpoint for retrieving a user's competition participation history.
/// </summary>
public class GetCompetitionHistoryEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/User/{id}/competition-history", [Authorize] async (
            IMediator mediator,
            string id,
            int skip = 0,
            int take = 10) =>
        {
            var query = new GetCompetitionHistoryQuery(id, skip, take);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetCompetitionHistory")
        .WithTags("Users")
        .Produces<GetCompetitionHistoryResult>()
        .Produces(401)
        .Produces(403)
        .Produces(404);
    }
}
