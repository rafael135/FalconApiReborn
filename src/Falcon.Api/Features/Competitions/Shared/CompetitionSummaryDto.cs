using Falcon.Api.Features.Exercises.Shared;

namespace Falcon.Api.Features.Competitions.Shared;

/// <summary>
/// Summary information about a competition.
/// </summary>
public record CompetitionSummaryDto(
    Guid Id,
    string Name,
    Falcon.Core.Domain.Competitions.CompetitionStatus Status,
    DateTime StartTime,
    DateTime? EndTime,
    int RegisteredGroupsCount
);
