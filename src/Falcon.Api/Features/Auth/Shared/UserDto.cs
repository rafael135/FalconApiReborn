namespace Falcon.Api.Features.Auth.Shared;

/// <summary>
/// DTO básico de usuário retornado após autenticação/registro.
/// </summary>
/// <param name="Id">Identificador do usuário.</param>
/// <param name="Name">Nome completo.</param>
/// <param name="Email">E-mail do usuário.</param>
/// <param name="Ra">Registro Acadêmico (RA).</param>
/// <param name="Role">Papel do usuário (Student, Teacher, Admin).</param>
/// <param name="Group">Informações do grupo do usuário, se houver.</param>
public record UserDto(string Id, string Name, string Email, string Ra, string Role, GroupDto? Group);