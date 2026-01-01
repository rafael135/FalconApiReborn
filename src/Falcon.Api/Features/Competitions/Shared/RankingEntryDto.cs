namespace Falcon.Api.Features.Competitions.Shared;

/// <summary>
/// Represents a group's ranking entry in a competition.
/// </summary>
public record RankingEntryDto(
    Guid GroupId,
    string GroupName,
    double Points,
    double Penalty,
    int RankOrder,
    int SolvedExercises,
    DateTime? LastSubmissionTime
);
