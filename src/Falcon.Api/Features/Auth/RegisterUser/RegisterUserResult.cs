namespace Falcon.Api.Features.Auth.RegisterUser;

/// <summary>
/// Resultado retornado após registro de usuário.
/// </summary>
/// <param name="Id">Id do usuário.</param>
/// <param name="Name">Nome do usuário.</param>
/// <param name="Email">E-mail do usuário.</param>
/// <param name="Ra">Registro Acadêmico.</param>
/// <param name="Role">Papel do usuário.</param>
/// <param name="Token">Token de autenticação JWT.</param>
/// <param name="EmailConfirmed">Indica se o e-mail foi confirmado.</param>
/// <param name="joinYear">Ano de ingresso (opcional).</param>
public record RegisterUserResult(
    string Id,
    string Name,
    string Email,
    string Ra,
    string Role,
    string Token,
    bool EmailConfirmed,
    int? joinYear
);
