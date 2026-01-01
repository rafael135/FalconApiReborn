namespace Falcon.Api.Features.Competitions.Shared;

/// <summary>
/// Represents a group's registration in a competition.
/// </summary>
public record GroupInCompetitionDto(
    Guid GroupId,
    Guid CompetitionId,
    DateTime CreatedOn,
    bool Blocked
);
