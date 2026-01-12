using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Admin.GetUsers;

/// <summary>
/// Endpoint for getting all users (Admin only).
/// </summary>
public class GetUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/Admin/users", [Authorize(Roles = "Admin,Teacher")] async (
            IMediator mediator,
            string? role,
            string? search,
            int skip = 0,
            int take = 50) =>
        {
            var query = new GetUsersQuery(role, search, skip, take);
            var result = await mediator.Send(query);
            return Results.Ok(result);
        })
        .WithName("GetUsers")
        .WithTags("Admin")
        .WithSummary("Get a paginated list of users.")
        .WithDescription("Returns users filtered by role and search text; requires Admin or Teacher role.")
        .Produces<GetUsersResult>();
    }
}
