namespace Falcon.Api.Features.Auth.Shared;

/// <summary>
/// DTO que representa um convite para um usuário entrar em um grupo.
/// </summary>
/// <param name="Id">Identificador do convite.</param>
/// <param name="UserId">Id do usuário convidado.</param>
/// <param name="GroupId">Id do grupo.</param>
/// <param name="Accepted">Indica se o convite foi aceito.</param>
public record GroupInviteDto(Guid Id, string UserId, Guid GroupId, bool Accepted);
