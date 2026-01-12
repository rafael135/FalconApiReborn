namespace Falcon.Api.Features.Auth.Shared;

/// <summary>
/// DTO que representa um grupo e seus convites.
/// </summary>
/// <param name="Id">Identificador do grupo.</param>
/// <param name="Name">Nome do grupo.</param>
/// <param name="LeaderId">Id do usuário líder.</param>
/// <param name="Invites">Convites associados ao grupo.</param>
public record GroupDto(Guid Id, string Name, string LeaderId, List<GroupInviteDto> Invites);