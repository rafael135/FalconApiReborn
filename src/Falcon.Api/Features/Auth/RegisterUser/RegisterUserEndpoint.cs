using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Auth.RegisterUser;

/// <summary>
/// Registers a new user and returns an authentication token.
/// </summary>
/// <remarks>
/// Accepts a <see cref="RegisterUserCommand"/> in the request body. On success, an HttpOnly cookie
/// named `CompetitionAuthToken` is appended and the user information with token is returned.
/// Example request: { "name": "Alice", "email": "alice@example.com", "ra": "12345", "password": "password123" }
/// </remarks>
public class RegisterUserEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the register endpoint into the specified <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="app">The route builder to add the endpoint to.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Auth/register", async (IMediator mediator, HttpContext httpContext, [FromBody] RegisterUserCommand command) =>
        {
            var result = await mediator.Send(command);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(1),
                SameSite = SameSiteMode.Lax,
                Path = "/",
            };
            httpContext.Response.Cookies.Append("CompetitionAuthToken", result.Token, cookieOptions);

            return Results.Ok(new { user = result, token = result.Token });
        })
        .WithName("RegisterUser")
        .WithTags("Auth")
        .WithSummary("Register a new user (student or teacher).")
        .WithDescription("Creates a new user account. For teacher role, supply a valid access code. Returns the created user and an authentication token; sets an HttpOnly cookie named 'CompetitionAuthToken'.")
        .Produces<RegisterUserResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status422UnprocessableEntity);
    }
}
