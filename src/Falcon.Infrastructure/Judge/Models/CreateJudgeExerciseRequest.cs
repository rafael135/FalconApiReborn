using System.Text.Json.Serialization;

namespace Falcon.Infrastructure.Judge.Models;

/// <summary>
/// Request to create an exercise in Judge API.
/// </summary>
public class CreateJudgeExerciseRequest
{
    /// <summary>
    /// The exercise name in the Judge API.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The exercise description.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The list of input dataset strings for each test case.
    /// </summary>
    [JsonPropertyName("data_entry")]
    public List<string> DataEntry { get; set; } = new();

    /// <summary>
    /// The expected output dataset strings for each test case.
    /// </summary>
    [JsonPropertyName("data_output")]
    public List<string> DataOutput { get; set; } = new();

    /// <summary>
    /// Optional human readable description of input fields.
    /// </summary>
    [JsonPropertyName("entry_description")]
    public string EntryDescription { get; set; } = string.Empty;

    /// <summary>
    /// Optional human readable description of output fields.
    /// </summary>
    [JsonPropertyName("output_description")]
    public string OutputDescription { get; set; } = string.Empty;
}
