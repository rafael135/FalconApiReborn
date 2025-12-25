namespace Falcon.Api.Features.Auth.Shared;

public record GroupDto(Guid Id, string Name, string LeaderId, List<GroupInviteDto> Invites);