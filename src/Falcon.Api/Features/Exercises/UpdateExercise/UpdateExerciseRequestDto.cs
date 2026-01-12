using System.Text.Json.Serialization;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

/// <summary>
/// DTO com metadados para atualizar um exercício.
/// </summary>
/// <remarks>
/// Use este DTO como parte de um formulário multipart/form-data onde o campo `metadata` contém
/// o JSON serializado com a estrutura abaixo e o campo `file` é opcional para um arquivo anexado.
/// Exemplo de metadata JSON:
/// <code>
/// {
///   "id":"00000000-0000-0000-0000-000000000000",
///   "exerciseTypeId": 1,
///   "title":"Exemplo",
///   "description":"Descrição opcional",
///   "inputs": [{ "orderId": 1, "input": "1 2" }],
///   "outputs": [{ "orderId": 1, "output": "3" }]
/// }
/// </code>
/// </remarks>
public class UpdateExerciseRequestDto
{
    /// <summary>
    /// Identificador do exercício.
    /// </summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Identificador do tipo do exercício.
    /// </summary>
    [JsonPropertyName("exerciseTypeId")]
    public int ExerciseTypeId { get; set; }

    /// <summary>
    /// Título do exercício.
    /// </summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Descrição opcional do exercício.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Lista de entradas (inputs) do exercício.
    /// </summary>
    [JsonPropertyName("inputs")]
    public List<UpdateExerciseInputDto> Inputs { get; set; } = new();

    /// <summary>
    /// Lista de saídas (outputs) esperadas do exercício.
    /// </summary>
    [JsonPropertyName("outputs")]
    public List<UpdateExerciseOutputDto> Outputs { get; set; } = new();
} 

/// <summary>
/// Representa uma entrada (input) do exercício para atualização.
/// </summary>
public class UpdateExerciseInputDto
{
    /// <summary>
    /// Identificador do input (opcional para novos inputs).
    /// </summary>
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    /// <summary>
    /// Posição/ordem deste input.
    /// </summary>
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    /// <summary>
    /// Conteúdo do input.
    /// </summary>
    [JsonPropertyName("input")]
    public string Input { get; set; } = string.Empty;
}

/// <summary>
/// Representa uma saída (output) esperada do exercício para atualização.
/// </summary>
public class UpdateExerciseOutputDto
{
    /// <summary>
    /// Identificador do output (opcional para novos outputs).
    /// </summary>
    [JsonPropertyName("id")]
    public Guid? Id { get; set; }

    /// <summary>
    /// Posição/ordem deste output.
    /// </summary>
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }

    /// <summary>
    /// Conteúdo do output esperado.
    /// </summary>
    [JsonPropertyName("output")]
    public string Output { get; set; } = string.Empty;
}  
