using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Users.UpdateUser;

/// <summary>
/// Endpoint for updating user profile information.
/// </summary>
/// <remarks>
/// Accepts an <see cref="UpdateUserCommand"/> in the request body. Validates that the route `id` matches `command.UserId`.
/// Example request: { "userId": "...", "name": "Alice", "email": "alice@example.com", "ra": "12345", "newPassword": "newPassword123", "currentPassword": "currentPassword" }
/// Example response: { "user": { "id":"...", "name":"Alice", "email":"alice@example.com" } }
/// </remarks>
public class UpdateUserEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/User/{id}", [Authorize] async (
            IMediator mediator,
            string id,
            [FromBody] UpdateUserCommand command) =>
        {
            // Validate that route ID matches command ID
            if (id != command.UserId)
            {
                return Results.BadRequest(new
                {
                    error = "Route id does not match command.UserId"
                });
            }

            var result = await mediator.Send(command);
            return Results.Ok(result.User);
        })
        .WithName("UpdateUser")
        .WithTags("Users")
        .WithSummary("Update user profile.")
        .WithDescription("Updates user profile information. Requires authentication; the route id must match command.UserId.")
        .Produces<Shared.UserDetailDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound);
    }
}
