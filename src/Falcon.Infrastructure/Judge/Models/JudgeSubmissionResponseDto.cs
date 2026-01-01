using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Response from Judge API after submission.
/// </summary>
public class JudgeSubmissionResponseDto
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("execution_time")]
    public double ExecutionTime { get; set; }
}
