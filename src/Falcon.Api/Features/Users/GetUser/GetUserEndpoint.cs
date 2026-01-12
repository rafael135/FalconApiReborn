using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Users.GetUser;

/// <summary>
/// Endpoint for retrieving detailed user information.
/// </summary>
/// <remarks>
/// Returns detailed user information. Requires authentication; only the user themselves or an Admin can access this endpoint.
/// Example response: { "id": "...", "name": "Alice", "email": "alice@example.com", "ra": "12345", "roles": ["Student"] }
/// </remarks>
public class GetUserEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/User/{id}", [Authorize] async (IMediator mediator, string id) =>
        {
            var query = new GetUserQuery(id);
            var result = await mediator.Send(query);
            return Results.Ok(result.User);
        })
        .WithName("GetUser")
        .WithTags("Users")
        .Produces<Shared.UserDetailDto>()
        .Produces(401)
        .Produces(403)
        .Produces(404);
    }
}
