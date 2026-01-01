using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Request to create an exercise in Judge API.
/// </summary>
public class CreateJudgeExerciseRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("data_entry")]
    public List<string> DataEntry { get; set; } = new();

    [JsonPropertyName("data_output")]
    public List<string> DataOutput { get; set; } = new();

    [JsonPropertyName("entry_description")]
    public string EntryDescription { get; set; } = string.Empty;

    [JsonPropertyName("output_description")]
    public string OutputDescription { get; set; } = string.Empty;
}
