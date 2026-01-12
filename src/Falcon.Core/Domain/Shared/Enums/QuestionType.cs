namespace Falcon.Core.Domain.Shared.Enums;

/// <summary>
/// Types of questions asked in competitions or exercises.
/// </summary>
public enum QuestionType
{
    /// <summary>General question not specific to a problem.</summary>
    General = 1,
    /// <summary>Technical question about the platform or judge.</summary>
    Technical = 2,
    /// <summary>Clarification request regarding problem statement.</summary>
    Clarification = 3,
    /// <summary>Bug report regarding the exercise or system.</summary>
    BugReport = 4
}
