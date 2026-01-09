namespace Falcon.Api.Features.Users.Shared;

/// <summary>
/// Represents a user's participation in a competition with detailed statistics.
/// </summary>
/// <param name="CompetitionId">The unique identifier of the competition.</param>
/// <param name="CompetitionName">The name of the competition.</param>
/// <param name="StartTime">The date and time when the competition started.</param>
/// <param name="EndTime">An optional property of type DateTime representing when the competition ended.</param>
/// <param name="Status">The current status of the competition.</param>
/// <param name="GroupId">The unique identifier of the group the user participated with.</param>
/// <param name="GroupName">The name of the group.</param>
/// <param name="FinalRanking">An optional property of type int representing the final ranking position achieved.</param>
/// <param name="TotalPoints">The total points scored by the group.</param>
/// <param name="Penalty">The total penalty time incurred.</param>
/// <param name="ExercisesSolved">The number of exercises successfully solved.</param>
/// <param name="TotalSubmissions">The total number of submissions made.</param>
public record CompetitionHistoryDto(
    Guid CompetitionId,
    string CompetitionName,
    DateTime StartTime,
    DateTime? EndTime,
    string Status,
    Guid GroupId,
    string GroupName,
    int? FinalRanking,
    double TotalPoints,
    double Penalty,
    int ExercisesSolved,
    int TotalSubmissions
);
