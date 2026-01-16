using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Request to update an exercise in Judge API.
/// </summary>
public class UpdateJudgeExerciseRequest
{
    /// <summary>
    /// The UUID of the problem to update in Judge.
    /// </summary>
    [JsonPropertyName("problem_id")]
    public string ProblemId { get; set; } = string.Empty;

    /// <summary>
    /// The updated exercise name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The updated exercise description.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Array of input dataset strings for the test cases.
    /// </summary>
    [JsonPropertyName("data_entry")]
    public string[] DataEntry { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Array of expected output dataset strings for the test cases.
    /// </summary>
    [JsonPropertyName("data_output")]
    public string[] DataOutput { get; set; } = Array.Empty<string>();
}
