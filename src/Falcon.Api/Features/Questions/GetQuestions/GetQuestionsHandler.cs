using Falcon.Api.Features.Questions.Shared;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Questions.GetQuestions;

/// <summary>
/// Handles the retrieval of paginated questions for a competition.
/// </summary>
public class GetQuestionsHandler : IRequestHandler<GetQuestionsQuery, GetQuestionsResult>
{
    private readonly FalconDbContext _dbContext;

    public GetQuestionsHandler(FalconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GetQuestionsResult> Handle(
        GetQuestionsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Start with base query filtered by competition
        var query = _dbContext
            .Questions.AsNoTracking()
            .Where(q => q.CompetitionId == request.CompetitionId);

        // Apply optional filters
        if (request.ExerciseId.HasValue)
        {
            query = query.Where(q => q.ExerciseId == request.ExerciseId.Value);
        }

        if (request.QuestionType.HasValue)
        {
            query = query.Where(q => (int)q.QuestionType == request.QuestionType.Value);
        }

        // Order by most recent first
        query = query.OrderByDescending(q => q.CreatedAt);

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply pagination and eager load navigation properties
        var questions = await query
            .Skip(request.Skip)
            .Take(request.Take)
            .Include(q => q.User)
            .ThenInclude(u => u.Group!)
            .Include(q => q.Answer!)
            .ThenInclude(a => a.User)
            .Include(q => q.Exercise)
            .Select(q => new QuestionDto(
                q.Id,
                q.CompetitionId,
                q.ExerciseId,
                q.UserId,
                q.User.Name,
                q.User.GroupId,
                q.User.Group != null ? q.User.Group.Name : null,
                q.Content,
                q.QuestionType,
                q.CreatedAt,
                q.Answer != null
                    ? new AnswerDto(
                        q.Answer.Id,
                        q.Answer.Content,
                        q.Answer.UserId,
                        q.Answer.User.Name,
                        q.Answer.CreatedAt
                    )
                    : null
            ))
            .ToListAsync(cancellationToken);

        return new GetQuestionsResult(questions, totalCount, request.Skip, request.Take);
    }
}
