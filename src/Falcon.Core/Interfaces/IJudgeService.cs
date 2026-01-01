using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Core.Interfaces;

/// <summary>
/// Service for interacting with the external Judge system for code evaluation.
/// </summary>
public interface IJudgeService
{
    /// <summary>
    /// Authenticates with the Judge API and retrieves an access token.
    /// </summary>
    /// <returns>The access token or null if authentication fails.</returns>
    Task<string?> AuthenticateJudgeAsync();

    /// <summary>
    /// Fetches a valid Judge token from cache or authenticates if needed.
    /// </summary>
    /// <returns>A valid token or null if unavailable.</returns>
    Task<string?> FetchJudgeTokenAsync();

    /// <summary>
    /// Creates an exercise in the Judge system.
    /// </summary>
    /// <param name="title">Exercise title.</param>
    /// <param name="description">Exercise description.</param>
    /// <param name="testCases">List of test cases.</param>
    /// <returns>The Judge UUID for the created exercise.</returns>
    Task<string?> CreateExerciseAsync(string title, string description, List<TestCase> testCases);

    /// <summary>
    /// Submits code to the Judge system for evaluation.
    /// </summary>
    /// <param name="code">The source code to evaluate.</param>
    /// <param name="language">Programming language.</param>
    /// <param name="exerciseUuid">The exercise UUID in the Judge system.</param>
    /// <returns>The submission result.</returns>
    Task<JudgeSubmissionResult> SubmitCodeAsync(string code, string language, string exerciseUuid);

    /// <summary>
    /// Gets an exercise from Judge by its UUID.
    /// </summary>
    /// <param name="judgeUuid">The Judge UUID of the exercise.</param>
    /// <returns>Exercise information or null if not found.</returns>
    Task<JudgeExerciseInfo?> GetExerciseByUuidAsync(string judgeUuid);

    /// <summary>
    /// Updates an exercise in the Judge system.
    /// </summary>
    /// <param name="judgeUuid">The Judge UUID of the exercise.</param>
    /// <param name="title">Updated title.</param>
    /// <param name="description">Updated description.</param>
    /// <param name="testCases">Updated test cases.</param>
    /// <returns>True if update was successful.</returns>
    Task<bool> UpdateExerciseAsync(string judgeUuid, string title, string description, List<TestCase> testCases);
}

/// <summary>
/// Represents a test case for an exercise.
/// </summary>
public record TestCase(string Input, string ExpectedOutput);

/// <summary>
/// Result of a code submission to the Judge.
/// </summary>
public record JudgeSubmissionResult(
    string SubmissionId,
    JudgeSubmissionResponse Status,
    TimeSpan ExecutionTime,
    int MemoryUsed
);

/// <summary>
/// Status of a submission in the Judge system.
/// </summary>
public record JudgeSubmissionStatus(
    string SubmissionId,
    JudgeSubmissionResponse Status,
    string? ErrorMessage
);

/// <summary>
/// Information about an exercise in the Judge system.
/// </summary>
public record JudgeExerciseInfo(
    string Id,
    string Name,
    string Description,
    List<string> DataEntry,
    List<string> DataOutput
);
