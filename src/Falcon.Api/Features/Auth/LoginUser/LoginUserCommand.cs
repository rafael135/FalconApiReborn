using MediatR;

namespace Falcon.Api.Features.Auth.LoginUser;

/// <summary>
/// Comando para autenticar um usuário.
/// </summary>
/// <param name="RA">E-mail ou RA do usuário.</param>
/// <param name="Password">Senha do usuário.</param>
public record LoginUserCommand(string RA, string Password) : IRequest<LoginUserResult>; 
