using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Logs.GetUserLogs;

/// <summary>
/// Endpoint for getting a specific user's logs (Admin only).
/// </summary>
public class GetUserLogsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/Log/user/{userId}",
                [Authorize(Roles = "Admin")]
                async (IMediator mediator, string userId, int skip = 0, int take = 50) =>
                {
                    var query = new GetUserLogsQuery(userId, skip, take);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
            )
            .WithName("GetUserLogs")
            .WithTags("Logs")
            .WithSummary("Get logs for a specific user.")
            .WithDescription("Returns logs related to a specific user. Requires Admin role.")
            .Produces<GetUserLogsResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
