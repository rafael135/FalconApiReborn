using Falcon.Api.Features.Groups.Shared;

namespace Falcon.Api.Features.Groups.InviteUser;

/// <summary>
/// Result of inviting a user to a group.
/// </summary>
public record InviteUserResult(GroupInviteDto Invite);
