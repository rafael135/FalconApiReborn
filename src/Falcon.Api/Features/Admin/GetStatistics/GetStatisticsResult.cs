namespace Falcon.Api.Features.Admin.GetStatistics;

/// <summary>
/// System statistics aggregated for the admin dashboard.
/// </summary>
public record GetStatisticsResult(
    /// <summary>User-related statistics.</summary>
    UserStatistics Users,
    /// <summary>Total number of groups in the system.</summary>
    int TotalGroups,
    /// <summary>Competition-related statistics.</summary>
    CompetitionStatistics Competitions,
    /// <summary>Exercise-related statistics.</summary>
    ExerciseStatistics Exercises,
    /// <summary>Submission-related statistics.</summary>
    SubmissionStatistics Submissions
);

/// <summary>
/// Aggregated user counts.
/// </summary>
public record UserStatistics(
    /// <summary>Total number of students.</summary>
    int TotalStudents,
    /// <summary>Total number of teachers.</summary>
    int TotalTeachers,
    /// <summary>Total number of admins.</summary>
    int TotalAdmins,
    /// <summary>Total number of users.</summary>
    int TotalUsers
);

/// <summary>
/// Aggregated competition counts by state.
/// </summary>
public record CompetitionStatistics(
    /// <summary>Competitions pending start.</summary>
    int TotalPending,
    /// <summary>Competitions currently ongoing.</summary>
    int TotalOngoing,
    /// <summary>Competitions finished.</summary>
    int TotalFinished,
    /// <summary>Total competitions tracked.</summary>
    int TotalCompetitions
);

/// <summary>
/// Exercise metrics.
/// </summary>
public record ExerciseStatistics(
    /// <summary>Count of exercises grouped by type.</summary>
    Dictionary<string, int> ByType,
    /// <summary>Total number of exercises.</summary>
    int TotalExercises
);

/// <summary>
/// Submission metrics.
/// </summary>
public record SubmissionStatistics(
    /// <summary>Total submissions received.</summary>
    int TotalSubmissions,
    /// <summary>Number of accepted submissions.</summary>
    int AcceptedSubmissions,
    /// <summary>Acceptance rate (0..1).</summary>
    double AcceptanceRate
);
