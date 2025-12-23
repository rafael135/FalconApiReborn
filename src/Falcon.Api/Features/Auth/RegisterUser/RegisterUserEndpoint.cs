using Falcon.Api.Features.Auth.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Auth.RegisterUser;

[ApiController]
[Route("api/auth")] // Mantendo a rota original
public class RegisterUserEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public RegisterUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
    {
        // 1. O Controller delega tudo para o Handler
        var result = await _mediator.Send(command);

        // 2. Lógica de Apresentação (Cookies) fica aqui, pois é HTTP puro
        SetAuthCookie(result.Token);

        // 3. Retorna o JSON
        return Ok(new { user = result, token = result.Token });
    }

    private void SetAuthCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddDays(1),
            SameSite = SameSiteMode.Lax,
            Path = "/",
        };
        Response.Cookies.Append("CompetitionAuthToken", token, cookieOptions);
    }
}
