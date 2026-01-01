namespace Falcon.Api.Features.Questions.Shared;

public record AnswerDto(
    Guid Id,
    string Content,
    string UserId,
    string UserName,
    DateTime CreatedAt
);
