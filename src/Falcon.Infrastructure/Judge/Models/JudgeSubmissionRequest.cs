using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Request to submit code to Judge API.
/// </summary>
public class JudgeSubmissionRequest
{
    [JsonPropertyName("problem_id")]
    public string ProblemId { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("language_type")]
    public string LanguageType { get; set; } = string.Empty;
}
