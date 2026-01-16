using Falcon.Api.Extensions;
using Falcon.Api.Features.Users.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Users.GetCompetitionHistory;

/// <summary>
/// Endpoint for retrieving a user's competition participation history.
/// </summary>
/// <remarks>
/// Supports pagination via `skip` and `take`. Example response: { "history": [ { "competitionId": "...", "competitionName": "ICPC" } ], "totalCount": 12 }
/// </remarks>
public class GetCompetitionHistoryEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/User/{id}/competition-history",
                [Authorize]
                async (IMediator mediator, string id, int skip = 0, int take = 10) =>
                {
                    var query = new GetCompetitionHistoryQuery(id, skip, take);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
            )
            .WithName("GetCompetitionHistory")
            .WithTags("Users")
            .WithSummary("Get competition history for a user.")
            .WithDescription(
                "Returns a paginated list of competitions the specified user participated in. Requires authentication and authorization."
            )
            .Produces<GetCompetitionHistoryResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);
    }
}
