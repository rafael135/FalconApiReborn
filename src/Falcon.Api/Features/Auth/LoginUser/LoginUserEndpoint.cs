using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Auth.LoginUser;

[ApiController]
[Route("api/Auth")]
public class LoginUserEndpoint : ControllerBase
{
    private readonly IMediator _mediator;

    public LoginUserEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
    {
        // 1. O Controller delega tudo para o Handler
        var result = await _mediator.Send(command);

        // 2. Lógica de Apresentação (Cookies) fica aqui, pois é HTTP puro
        this.SetAuthCookie(result.Token);

        // 3. Retorna o JSON
        return Ok(new { user = result, token = result.Token });
    }
}
