using Falcon.Api.Features.Exercises.Shared;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Exercises.CreateExercise;

/// <summary>
/// DTO contendo metadados para criação de exercício.
/// </summary>
/// <remarks>
/// Exemplo de metadata para envio no campo `metadata` do formulário multipart:
/// <code>
/// {
///   "title":"Somar dois números",
///   "description":"Leia dois inteiros e imprima a soma",
///   "exerciseTypeId":1,
///   "estimatedTime":"00:10:00",
///   "inputs":[{"orderId":1,"input":"1 2"}],
///   "outputs":[{"orderId":1,"output":"3"}]
/// }
/// </code>
/// </remarks>
public class CreateExerciseRequestDto
{
    /// <summary>O título do exercício.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Descrição do exercício (opcional).</summary>
    public string? Description { get; set; }

    /// <summary>Tipo do exercício (ID).</summary>
    public int ExerciseTypeId { get; set; }

    /// <summary>Tempo estimado para resolver o exercício.</summary>
    public TimeSpan EstimatedTime { get; set; }

    /// <summary>Entradas do exercício.</summary>
    public List<ExerciseInputDto> Inputs { get; set; } = new();

    /// <summary>Saídas esperadas do exercício.</summary>
    public List<ExerciseOutputDto> Outputs { get; set; } = new();
}

/// <summary>Entrada de exemplo para o exercício.</summary>
public class ExerciseInputDto
{
    /// <summary>Ordem do input.</summary>
    public int OrderId { get; set; }
    /// <summary>Conteúdo do input.</summary>
    public string Input { get; set; } = string.Empty;
}

/// <summary>Saída de exemplo para o exercício.</summary>
public class ExerciseOutputDto
{
    /// <summary>Ordem do output.</summary>
    public int OrderId { get; set; }
    /// <summary>Conteúdo do output esperado.</summary>
    public string Output { get; set; } = string.Empty;
} 
