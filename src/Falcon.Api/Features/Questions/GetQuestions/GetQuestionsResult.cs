using Falcon.Api.Features.Questions.Shared;

namespace Falcon.Api.Features.Questions.GetQuestions;

/// <summary>
/// Result containing paginated questions.
/// </summary>
/// <param name="Questions">The list of questions for the current page.</param>
/// <param name="TotalCount">The total number of questions matching the filter criteria.</param>
/// <param name="Skip">The number of records skipped.</param>
/// <param name="Take">The number of records returned.</param>
public record GetQuestionsResult(List<QuestionDto> Questions, int TotalCount, int Skip, int Take);
