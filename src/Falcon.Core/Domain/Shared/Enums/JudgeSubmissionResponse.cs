namespace Falcon.Core.Domain.Shared.Enums;

/// <summary>
/// Possible results returned by the judge for a submission.
/// </summary>
public enum JudgeSubmissionResponse
{
    /// <summary>Submission is pending evaluation.</summary>
    Pending = 0,

    /// <summary>Submission was accepted.</summary>
    Accepted = 1,

    /// <summary>Submission produced a wrong answer.</summary>
    WrongAnswer = 2,

    /// <summary>Submission exceeded the time limit.</summary>
    TimeLimitExceeded = 3,

    /// <summary>Submission exceeded the memory limit.</summary>
    MemoryLimitExceeded = 4,

    /// <summary>Submission raised a runtime error.</summary>
    RuntimeError = 5,

    /// <summary>Submission failed to compile.</summary>
    CompilationError = 6,

    /// <summary>An internal error occurred during evaluation.</summary>
    InternalError = 7,

    /// <summary>Submission produced a presentation/formatting error.</summary>
    PresentationError = 8,

    /// <summary>Submission triggered a security policy violation.</summary>
    SecurityError = 9,
}
