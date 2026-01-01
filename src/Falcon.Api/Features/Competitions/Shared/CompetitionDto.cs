namespace Falcon.Api.Features.Competitions.Shared;

/// <summary>
/// Basic competition information returned after create/update operations.
/// </summary>
public record CompetitionDto(
    Guid Id,
    string Name,
    string Description,
    Falcon.Core.Domain.Competitions.CompetitionStatus Status,
    DateTime StartInscriptions,
    DateTime EndInscriptions,
    DateTime StartTime,
    DateTime? EndTime,
    int? MaxMembers,
    int? MaxExercises
);
