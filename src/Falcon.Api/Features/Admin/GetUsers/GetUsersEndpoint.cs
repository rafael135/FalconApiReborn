using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Admin.GetUsers;

/// <summary>
/// Endpoint for getting all users (Admin only).
/// </summary>
public static class GetUsersEndpoint
{
    public static IEndpointRouteBuilder MapGetUsers(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Admin/users", [Authorize(Roles = "Admin")] async (
            IMediator mediator,
            string? role,
            int skip = 0,
            int take = 50) =>
        {
            var query = new GetUsersQuery(role, skip, take);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetUsers")
        .WithTags("Admin")
        .Produces<GetUsersResult>();

        return app;
    }
}
