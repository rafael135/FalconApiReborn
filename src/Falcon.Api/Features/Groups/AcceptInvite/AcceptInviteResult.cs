using Falcon.Api.Features.Groups.Shared;

namespace Falcon.Api.Features.Groups.AcceptInvite;

/// <summary>
/// Result of accepting a group invitation.
/// </summary>
public record AcceptInviteResult(GroupDto Group);
