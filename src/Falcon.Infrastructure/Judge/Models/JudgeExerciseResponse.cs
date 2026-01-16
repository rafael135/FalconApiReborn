using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Response from Judge API when creating/getting an exercise.
/// </summary>
public class JudgeExerciseResponse
{
    /// <summary>
    /// The Judge-assigned exercise identifier (UUID).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The exercise name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The exercise description.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Input data set examples for test cases.
    /// </summary>
    [JsonPropertyName("data_entry")]
    public List<string> DataEntry { get; set; } = new();

    /// <summary>
    /// Expected output data set examples for test cases.
    /// </summary>
    [JsonPropertyName("data_output")]
    public List<string> DataOutput { get; set; } = new();

    /// <summary>
    /// Optional description of the input fields.
    /// </summary>
    [JsonPropertyName("entry_description")]
    public string EntryDescription { get; set; } = string.Empty;

    /// <summary>
    /// Optional description of the output fields.
    /// </summary>
    [JsonPropertyName("output_description")]
    public string OutputDescription { get; set; } = string.Empty;
}
