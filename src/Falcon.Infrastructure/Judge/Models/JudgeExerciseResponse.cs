using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Response from Judge API when creating/getting an exercise.
/// </summary>
public class JudgeExerciseResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

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
