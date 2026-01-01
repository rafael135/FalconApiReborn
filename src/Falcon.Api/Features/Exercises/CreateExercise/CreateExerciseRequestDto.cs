using Falcon.Api.Features.Exercises.Shared;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Exercises.CreateExercise;

public class CreateExerciseRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ExerciseTypeId { get; set; }
    public TimeSpan EstimatedTime { get; set; }
    public List<ExerciseInputDto> Inputs { get; set; } = new();
    public List<ExerciseOutputDto> Outputs { get; set; } = new();
}

public class ExerciseInputDto
{
    public int OrderId { get; set; }
    public string Input { get; set; } = string.Empty;
}

public class ExerciseOutputDto
{
    public int OrderId { get; set; }
    public string Output { get; set; } = string.Empty;
}
