namespace Falcon.Api.Features.Admin.GetStatistics;

/// <summary>
/// System statistics for admin dashboard.
/// </summary>
public record GetStatisticsResult(
    UserStatistics Users,
    int TotalGroups,
    CompetitionStatistics Competitions,
    ExerciseStatistics Exercises,
    SubmissionStatistics Submissions
);

public record UserStatistics(
    int TotalStudents,
    int TotalTeachers,
    int TotalAdmins,
    int TotalUsers
);

public record CompetitionStatistics(
    int TotalPending,
    int TotalOngoing,
    int TotalFinished,
    int TotalCompetitions
);

public record ExerciseStatistics(
    Dictionary<string, int> ByType,
    int TotalExercises
);

public record SubmissionStatistics(
    int TotalSubmissions,
    int AcceptedSubmissions,
    double AcceptanceRate
);
