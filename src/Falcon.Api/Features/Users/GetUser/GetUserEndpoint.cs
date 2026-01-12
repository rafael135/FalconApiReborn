using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

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
        .WithSummary("Get a user by id.")
        .WithDescription("Returns detailed user information. Requires authentication; only the user themselves or an Admin can access this endpoint.")
        .Produces<Shared.UserDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
