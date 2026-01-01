using System.Text.Json.Serialization;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

public class UpdateExerciseRequestDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("exerciseTypeId")]
    public int ExerciseTypeId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("inputs")]
    public List<UpdateExerciseInputDto> Inputs { get; set; } = new();

    [JsonPropertyName("outputs")]
    public List<UpdateExerciseOutputDto> Outputs { get; set; } = new();
}

public class UpdateExerciseInputDto
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("input")]
    public string Input { get; set; } = string.Empty;
}

public class UpdateExerciseOutputDto
{
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    [JsonPropertyName("output")]
    public string Output { get; set; } = string.Empty;
}
