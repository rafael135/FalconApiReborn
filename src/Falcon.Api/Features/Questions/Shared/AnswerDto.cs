namespace Falcon.Api.Features.Questions.Shared;

/// <summary>
/// Represents an answer to a question in the Q&A system.
/// </summary>
/// <param name="Id">Unique identifier for the answer.</param>
/// <param name="Content">The answer content.</param>
/// <param name="UserId">Identifier of the user who posted the answer.</param>
/// <param name="UserName">Display name of the answer author.</param>
/// <param name="CreatedAt">Timestamp when the answer was created.</param>
public record AnswerDto(
    Guid Id,
    string Content,
    string UserId,
    string UserName,
    DateTime CreatedAt
);
