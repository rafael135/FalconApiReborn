using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Auth.LoginUser;

/// <summary>
/// Authenticates a user and returns an authentication token.
/// </summary>
/// <remarks>
/// Accepts a <see cref="LoginUserCommand"/> in the request body and appends the HttpOnly cookie
/// `CompetitionAuthToken` on success. Example request: { "email": "alice@example.com", "password": "password123" }
/// </remarks>
public class LoginUserEndpoint : IEndpoint
{
    /// <summary>
    /// Maps the login endpoint into the provided <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    /// <param name="app">The route builder to add the endpoint to.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Auth/login", async (IMediator mediator, HttpContext httpContext, [FromBody] LoginUserCommand command) =>
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
        .WithName("LoginUser")
        .WithTags("Auth")
        .WithSummary("Authenticate a user and return tokens.")
        .WithDescription("Authenticates the user and sets an HttpOnly 'CompetitionAuthToken' cookie; returns user information and token.")
        .Produces<LoginUserResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status422UnprocessableEntity);
    }
}
