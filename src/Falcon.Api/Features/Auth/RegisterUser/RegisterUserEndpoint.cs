using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Auth.RegisterUser;

public class RegisterUserEndpoint : IEndpoint
{
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
        .WithTags("Auth");
    }
}
