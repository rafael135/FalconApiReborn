using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Users.DeleteUser;

/// <summary>
/// Endpoint for soft deleting a user.
/// </summary>
/// <remarks>
/// Performs a soft delete of the user. Requires authentication. Example response: { "message": "User soft-deleted" }
/// </remarks>
public class DeleteUserEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/api/User/{id}",
                [Authorize]
                async (IMediator mediator, string id) =>
                {
                    var command = new DeleteUserCommand(id);
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
            )
            .WithName("DeleteUser")
            .WithTags("Users")
            .WithSummary("Soft delete a user.")
            .WithDescription(
                "Performs a soft delete for the specified user. Requires authentication and appropriate authorization."
            )
            .Produces<DeleteUserResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status422UnprocessableEntity);
    }
}
