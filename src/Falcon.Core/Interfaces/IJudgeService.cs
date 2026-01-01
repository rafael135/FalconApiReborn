namespace Falcon.Core.Interfaces;

/// <summary>
/// Service for interacting with the external Judge system for code evaluation.
/// </summary>
public interface IJudgeService
{
    /// <summary>
    /// Creates an exercise in the Judge system.
    /// </summary>
    /// <param name="title">Exercise title.</param>
    /// <param name="description">Exercise description.</param>
    /// <param name="testCases">List of test cases.</param>
    /// <returns>The Judge UUID for the created exercise.</returns>
    Task<string> CreateExerciseAsync(string title, string description, List<TestCase> testCases);

    /// <summary>
    /// Submits code to the Judge system for evaluation.
    /// </summary>
    /// <param name="code">The source code to evaluate.</param>
    /// <param name="language">Programming language.</param>
    /// <param name="exerciseUuid">The exercise UUID in the Judge system.</param>
    /// <returns>The submission result.</returns>
    Task<JudgeSubmissionResult> SubmitCodeAsync(string code, string language, string exerciseUuid);

    /// <summary>
    /// Gets the status of a submission.
    /// </summary>
    /// <param name="submissionId">The submission ID.</param>
    /// <returns>The submission status.</returns>
    Task<JudgeSubmissionStatus> GetSubmissionStatusAsync(string submissionId);
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
/// Possible responses from the Judge system.
/// </summary>
public enum JudgeSubmissionResponse
{
    Accepted = 0,
    WrongAnswer = 1,
    TimeLimitExceeded = 2,
    MemoryLimitExceeded = 3,
    RuntimeError = 4,
    CompilationError = 5,
    Pending = 6,
    Judging = 7
}
