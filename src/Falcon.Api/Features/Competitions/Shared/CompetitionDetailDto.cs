using Falcon.Api.Features.Exercises.Shared;

namespace Falcon.Api.Features.Competitions.Shared;

/// <summary>
/// Detailed information about a competition including exercises and rankings.
/// </summary>
public record CompetitionDetailDto(
    Guid Id,
    string Name,
    string Description,
    Falcon.Core.Domain.Competitions.CompetitionStatus Status,
    DateTime StartInscriptions,
    DateTime EndInscriptions,
    DateTime StartTime,
    DateTime? EndTime,
    int? MaxMembers,
    int? MaxExercises,
    int? MaxSubmissionSize,
    TimeSpan? Duration,
    TimeSpan? SubmissionPenalty,
    List<ExerciseSummaryDto> Exercises,
    List<RankingEntryDto> Rankings
);
