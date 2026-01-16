using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Exercises;

/// <summary>
/// Represents a question that can be associated with an exercise or competition.
/// </summary>
public class Question : Entity
{
    /// <summary>Maximum allowed length for the question content.</summary>
    public const int MaxContentLength = 1000;

    /// <summary>Competition identifier this question belongs to.</summary>
    public Guid CompetitionId { get; private set; }
    /// <summary>Competition navigation property.</summary>
    public virtual Competitions.Competition Competition { get; private set; }

    /// <summary>Optional exercise identifier this question refers to.</summary>
    public Guid? ExerciseId { get; private set; }
    /// <summary>Optional exercise navigation property.</summary>
    public virtual Exercise? Exercise { get; private set; }

    /// <summary>Author user identifier.</summary>
    public string UserId { get; private set; }
    /// <summary>Author user navigation.</summary>
    public virtual User User { get; private set; }

    /// <summary>Identifier of the answer, if one exists.</summary>
    public Guid? AnswerId { get; private set; }
    /// <summary>Answer navigation property.</summary>
    public virtual Answer? Answer { get; private set; }

    /// <summary>Textual content of the question.</summary>
    public string Content { get; private set; }
    /// <summary>Type of question.</summary>
    public QuestionType QuestionType { get; private set; }
    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; private set; }
    /// <summary>Concurrency token.</summary>
    public byte[]? RowVersion { get; private set; }

#pragma warning disable CS8618
    protected Question() { }
#pragma warning restore CS8618

    /// <summary>Creates a new question associated with a competition (and optionally an exercise).</summary>
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
        if (content.Length > MaxContentLength)
            throw new ArgumentException($"Conteúdo não pode exceder {MaxContentLength} caracteres");

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

    /// <summary>Associates an answer with this question.</summary>
    /// <param name="answer">The answer to set.</param>
    public void SetAnswer(Answer answer)
    {
        if (answer == null)
            throw new ArgumentNullException(nameof(answer));

        Answer = answer;
        AnswerId = answer.Id;
    }
}
