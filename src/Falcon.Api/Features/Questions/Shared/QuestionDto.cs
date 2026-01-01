using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Api.Features.Questions.Shared;

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
