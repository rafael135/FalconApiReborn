using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Auth.LoginUser;

/// <summary>
/// Autentica um usuário e retorna token de autenticação.
/// </summary>
/// <remarks>
/// Recebe um <see cref="LoginUserCommand"/> no body e grava cookie HttpOnly `CompetitionAuthToken` em caso de sucesso.
/// Exemplo request: { "email": "alice@example.com", "password": "senha123" }
/// </remarks>
public class LoginUserEndpoint : IEndpoint
{
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
        .WithTags("Auth");
    }
}
