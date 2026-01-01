namespace Falcon.Api.Features.Groups.Shared;

/// <summary>
/// Detailed information about a group.
/// </summary>
public record GroupDto(
    Guid Id,
    string Name,
    string LeaderId,
    List<UserSummaryDto> Members,
    List<GroupInviteDto> Invites
);
