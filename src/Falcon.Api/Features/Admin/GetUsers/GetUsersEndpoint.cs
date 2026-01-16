using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Admin.GetUsers;

/// <summary>
/// Endpoint for getting all users (Admin or Teacher).
/// </summary>
public class GetUsersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/Admin/users",
                [Authorize(Roles = "Admin,Teacher")]
                async (
                    IMediator mediator,
                    [FromQuery(Name = "role")] string? role,
                    [FromQuery(Name = "search")] string? search,
                    int skip = 0,
                    int take = 50
                ) =>
                {
                    var query = new GetUsersQuery(role, search, skip, take);
                    var result = await mediator.Send(query);
                    return Results.Ok(result);
                }
            )
            .WithName("GetUsers")
            .WithTags("Admin")
            .WithSummary("Get a paginated list of users.")
            .WithDescription(
                "Returns users filtered by role and search text; requires Admin or Teacher role."
            )
            .Produces<GetUsersResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
