using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Auth.RegisterUser;

/// <summary>
/// Registra um novo usuário e retorna um token de autenticação.
/// </summary>
/// <remarks>
/// Recebe um <see cref="RegisterUserCommand"/> no body. Em caso de sucesso,
/// grava cookie HttpOnly `CompetitionAuthToken` e retorna informação do usuário e token.
/// Exemplo request: { "name": "Alice", "email": "alice@example.com", "ra": "12345", "password": "senha123" }
/// </remarks>
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
