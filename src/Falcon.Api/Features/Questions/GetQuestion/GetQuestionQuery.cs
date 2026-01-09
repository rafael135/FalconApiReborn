using Falcon.Api.Features.Questions.Shared;
using MediatR;

namespace Falcon.Api.Features.Questions.GetQuestion;

/// <summary>
/// Query to retrieve a single question by its ID.
/// </summary>
/// <param name="Id">The ID of the question to retrieve.</param>
public record GetQuestionQuery(Guid Id) : IRequest<QuestionDto>;
