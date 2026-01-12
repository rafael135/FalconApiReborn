using MediatR;

namespace Falcon.Api.Features.Auth.RegisterUser;

/// <summary>
/// Comando para registrar um novo usuário.
/// </summary>
/// <param name="Ra">Registro Acadêmico (RA) do aluno.</param>
/// <param name="Name">Nome completo do usuário.</param>
/// <param name="Email">E-mail do usuário (usado como UserName).</param>
/// <param name="Role">Papel desejado (Student|Teacher).</param>
/// <param name="Password">Senha em texto plano (será hasheada).</param>
/// <param name="Department">Departamento (opcional).</param>
/// <param name="AccessCode">Código necessário para registro de professor (opcional).</param>
/// <param name="joinYear">Ano de ingresso (opcional).</param>
public record RegisterUserCommand(
    string Ra,
    string Name,
    string Email,
    string Role,
    string Password,
    string? Department = null,
    string? AccessCode = null,
    int? joinYear = null
) : IRequest<RegisterUserResult>; 
