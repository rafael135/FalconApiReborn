namespace Falcon.Api.Features.Groups.RemoveMember;

/// <summary>
/// Result of removing a member from a group.
/// </summary>
public record RemoveMemberResult(bool Success, string Message);
