using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Auth.RegisterUser;

[ApiController]
[Route("api/Auth")]
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
        this.SetAuthCookie(result.Token);

        // 3. Retorna o JSON
        return Ok(new { user = result, token = result.Token });
    }
}
