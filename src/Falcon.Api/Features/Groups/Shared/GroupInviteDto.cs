namespace Falcon.Api.Features.Groups.Shared;

/// <summary>
/// Information about a group invitation.
/// </summary>
public record GroupInviteDto(Guid Id, string UserId, Guid GroupId, bool Accepted);
