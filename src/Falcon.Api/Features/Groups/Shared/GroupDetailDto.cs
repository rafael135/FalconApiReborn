namespace Falcon.Api.Features.Groups.Shared;

/// <summary>
/// Detailed information about a group including timestamps.
/// </summary>
public record GroupDetailDto(
    Guid Id,
    string Name,
    string LeaderId,
    List<UserSummaryDto> Members,
    List<GroupInviteDto> Invites,
    DateTime? LastCompetitionDate
);
