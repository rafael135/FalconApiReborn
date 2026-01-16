using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Request to submit code to Judge API.
/// </summary>
public class JudgeSubmissionRequest
{
    /// <summary>
    /// The problem UUID to which code will be submitted.
    /// </summary>
    [JsonPropertyName("problem_id")]
    public string ProblemId { get; set; } = string.Empty;

    /// <summary>
    /// The source code content to submit.
    /// </summary>
    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// The language identifier for the submission (e.g., "py", "cs").
    /// </summary>
    [JsonPropertyName("language_type")]
    public string LanguageType { get; set; } = string.Empty;
}
