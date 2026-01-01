namespace Falcon.Core.Domain.Shared.Enums;

public enum JudgeSubmissionResponse
{
    Pending = 0,
    Accepted = 1,
    WrongAnswer = 2,
    TimeLimitExceeded = 3,
    MemoryLimitExceeded = 4,
    RuntimeError = 5,
    CompilationError = 6,
    InternalError = 7
}
