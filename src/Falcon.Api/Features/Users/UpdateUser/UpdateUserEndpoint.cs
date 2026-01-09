using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Users.UpdateUser;

/// <summary>
/// Endpoint for updating user profile information.
/// </summary>
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
                    error = "O ID da rota não corresponde ao ID do comando"
                });
            }

            var result = await mediator.Send(command);
            return Results.Ok(result.User);
        })
        .WithName("UpdateUser")
        .WithTags("Users")
        .Produces<Shared.UserDetailDto>()
        .Produces(400)
        .Produces(401)
        .Produces(403)
        .Produces(404);
    }
}
