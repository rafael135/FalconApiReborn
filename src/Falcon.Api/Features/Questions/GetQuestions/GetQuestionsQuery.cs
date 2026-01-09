using MediatR;

namespace Falcon.Api.Features.Questions.GetQuestions;

/// <summary>
/// Query to retrieve a paginated list of questions for a specific competition.
/// </summary>
/// <param name="CompetitionId">The ID of the competition to filter by.</param>
/// <param name="ExerciseId">An optional property of type Guid. Filter questions by specific exercise.</param>
/// <param name="QuestionType">An optional property of type int. Filter questions by type (1=General, 2=Technical, 3=Clarification, 4=BugReport).</param>
/// <param name="Skip">The number of records to skip for pagination. Default is 0.</param>
/// <param name="Take">The number of records to take for pagination. Default is 10.</param>
public record GetQuestionsQuery(
    Guid CompetitionId,
    Guid? ExerciseId = null,
    int? QuestionType = null,
    int Skip = 0,
    int Take = 10
) : IRequest<GetQuestionsResult>;
