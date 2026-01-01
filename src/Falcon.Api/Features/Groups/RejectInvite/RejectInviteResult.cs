namespace Falcon.Api.Features.Groups.RejectInvite;

/// <summary>
/// Result of rejecting a group invitation.
/// </summary>
public record RejectInviteResult(bool Success, string Message);
