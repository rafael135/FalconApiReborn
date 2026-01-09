using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace Falcon.Api.Features.Users.DeleteUser;

/// <summary>
/// Endpoint for soft deleting a user.
/// </summary>
public class DeleteUserEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the endpoint to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/User/{id}", [Authorize] async (IMediator mediator, string id) =>
        {
            var command = new DeleteUserCommand(id);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("DeleteUser")
        .WithTags("Users")
        .Produces<DeleteUserResult>()
        .Produces(401)
        .Produces(403)
        .Produces(404)
        .Produces(422);
    }
}
