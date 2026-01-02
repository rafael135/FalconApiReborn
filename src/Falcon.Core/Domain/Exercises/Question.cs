using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Exercises;

/// <summary>
/// Represents a question that can be associated with an exercise or competition.
/// </summary>
public class Question : Entity
{
    public Guid CompetitionId { get; private set; }
    public virtual Competitions.Competition Competition { get; private set; }

    public Guid? ExerciseId { get; private set; }
    public virtual Exercise? Exercise { get; private set; }

    public string UserId { get; private set; }
    public virtual User User { get; private set; }

    public Guid? AnswerId { get; private set; }
    public virtual Answer? Answer { get; private set; }

    public string Content { get; private set; }
    public QuestionType QuestionType { get; private set; }
    public DateTime CreatedAt { get; private set; }

#pragma warning disable CS8618
    protected Question() { }
#pragma warning restore CS8618

    public Question(
        Competitions.Competition competition,
        User user,
        string content,
        QuestionType questionType,
        Exercise? exercise = null)
    {
        if (competition == null)
            throw new ArgumentNullException(nameof(competition));
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("O conteúdo da questão não pode estar vazio");

        Competition = competition;
        CompetitionId = competition.Id;
        User = user;
        UserId = user.Id;
        Content = content;
        QuestionType = questionType;
        Exercise = exercise;
        ExerciseId = exercise?.Id;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetAnswer(Answer answer)
    {
        if (answer == null)
            throw new ArgumentNullException(nameof(answer));

        Answer = answer;
        AnswerId = answer.Id;
    }
}
