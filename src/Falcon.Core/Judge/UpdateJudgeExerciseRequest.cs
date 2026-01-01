using System.Text.Json.Serialization;

namespace Falcon.Core.Judge;

/// <summary>
/// Request to update an exercise in Judge API.
/// </summary>
public class UpdateJudgeExerciseRequest
{
    [JsonPropertyName("problem_id")]
    public string ProblemId { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("data_entry")]
    public string[] DataEntry { get; set; } = Array.Empty<string>();

    [JsonPropertyName("data_output")]
    public string[] DataOutput { get; set; } = Array.Empty<string>();
}
