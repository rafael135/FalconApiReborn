using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Api.Features.Questions.Shared;

/// <summary>
/// DTO que representa uma pergunta de competição/exercício.
/// </summary>
/// <param name="Id">Identificador da pergunta.</param>
/// <param name="CompetitionId">Identificador da competição.</param>
/// <param name="ExerciseId">Identificador do exercício (opcional).</param>
/// <param name="UserId">Id do autor da pergunta.</param>
/// <param name="UserName">Nome do autor.</param>
/// <param name="GroupId">Id do grupo autor (opcional).</param>
/// <param name="GroupName">Nome do grupo (opcional).</param>
/// <param name="Content">Conteúdo da pergunta.</param>
/// <param name="QuestionType">Tipo de pergunta (enum).</param>
/// <param name="CreatedAt">Data de criação.</param>
/// <param name="Answer">Resposta (se houver).</param>
public record QuestionDto(
    Guid Id,
    Guid CompetitionId,
    Guid? ExerciseId,
    string UserId,
    string UserName,
    Guid? GroupId,
    string? GroupName,
    string Content,
    QuestionType QuestionType,
    DateTime CreatedAt,
    AnswerDto? Answer
);
