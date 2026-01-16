namespace Falcon.Core.Domain.Shared.Enums;

/// <summary>
/// Log action types used for auditing and analytics.
/// </summary>
public enum LogType
{
    /// <summary>User logged in.</summary>
    Login = 1,

    /// <summary>User logged out.</summary>
    Logout = 2,

    /// <summary>User submitted an exercise attempt.</summary>
    SubmitExercise = 3,

    /// <summary>User joined a competition.</summary>
    JoinCompetition = 4,

    /// <summary>User left a competition.</summary>
    LeaveCompetition = 5,

    /// <summary>Group was created.</summary>
    CreateGroup = 6,

    /// <summary>User joined a group.</summary>
    JoinGroup = 7,

    /// <summary>User left a group.</summary>
    LeaveGroup = 8,

    /// <summary>User viewed a competition ranking.</summary>
    ViewRanking = 9,

    /// <summary>User viewed an exercise.</summary>
    ViewExercise = 10,

    /// <summary>User downloaded a file.</summary>
    DownloadFile = 11,

    /// <summary>User sent a question.</summary>
    QuestionSent = 12,

    /// <summary>An answer was provided.</summary>
    AnswerGiven = 13,

    /// <summary>User deleted a user (admin action).</summary>
    DeleteUser = 14,

    /// <summary>An answer was updated.</summary>
    AnswerUpdated = 16,
}
