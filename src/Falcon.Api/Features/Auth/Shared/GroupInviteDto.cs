namespace Falcon.Api.Features.Auth.Shared;

public record GroupInviteDto(Guid Id, string UserId, Guid GroupId, bool Accepted);