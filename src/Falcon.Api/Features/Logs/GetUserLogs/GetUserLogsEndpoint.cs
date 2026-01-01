using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Logs.GetUserLogs;

/// <summary>
/// Endpoint for getting a specific user's logs (Admin only).
/// </summary>
public static class GetUserLogsEndpoint
{
    public static IEndpointRouteBuilder MapGetUserLogs(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Log/user/{userId}", [Authorize(Roles = "Admin")] async (
            IMediator mediator,
            string userId,
            int skip = 0,
            int take = 50) =>
        {
            var query = new GetUserLogsQuery(userId, skip, take);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetUserLogs")
        .WithTags("Logs")
        .Produces<GetUserLogsResult>();

        return app;
    }
}
