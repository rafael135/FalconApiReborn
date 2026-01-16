using Falcon.Api.Features.Questions.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Questions.GetQuestion;

/// <summary>
/// Handles the retrieval of a single question by ID.
/// </summary>
public class GetQuestionHandler : IRequestHandler<GetQuestionQuery, QuestionDto>
{
    private readonly FalconDbContext _dbContext;

    public GetQuestionHandler(FalconDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuestionDto> Handle(
        GetQuestionQuery request,
        CancellationToken cancellationToken
    )
    {
        var question = await _dbContext
            .Questions.AsNoTracking()
            .Include(q => q.User)
            .ThenInclude(u => u.Group!)
            .Include(q => q.Answer!)
            .ThenInclude(a => a.User)
            .Include(q => q.Exercise)
            .Where(q => q.Id == request.Id)
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
            .FirstOrDefaultAsync(cancellationToken);

        if (question == null)
        {
            throw new NotFoundException(nameof(Core.Domain.Exercises.Question), request.Id);
        }

        return question;
    }
}
