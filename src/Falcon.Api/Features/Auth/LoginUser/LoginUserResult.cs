using Falcon.Api.Features.Auth.Shared;

namespace Falcon.Api.Features.Auth.LoginUser;

/// <summary>
/// Resultado retornado após autenticação.
/// </summary>
/// <param name="User">Informações básicas do usuário.</param>
/// <param name="Token">Token de autenticação JWT.</param>
public record LoginUserResult(UserDto User, string Token);
