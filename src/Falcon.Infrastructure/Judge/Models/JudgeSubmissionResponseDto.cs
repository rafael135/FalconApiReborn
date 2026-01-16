using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Response from Judge API after submission.
/// </summary>
public class JudgeSubmissionResponseDto
{
    /// <summary>
    /// The status string returned by Judge (e.g., "ACCEPTED", "WRONG ANSWER").
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// The execution time in milliseconds reported by Judge.
    /// </summary>
    [JsonPropertyName("execution_time")]
    public double ExecutionTime { get; set; }
}
