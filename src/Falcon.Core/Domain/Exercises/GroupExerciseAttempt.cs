using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Core.Domain.Exercises;

/// <summary>
/// Represents an attempt where a group submits code to solve an exercise within a competition.
/// </summary>
public class GroupExerciseAttempt : Entity
{
    /// <summary>Identifier of the linked exercise.</summary>
    public Guid ExerciseId { get; private set; }

    /// <summary>Exercise navigation property.</summary>
    public virtual Exercise Exercise { get; private set; }

    /// <summary>Identifier of the submitting group.</summary>
    public Guid GroupId { get; private set; }

    /// <summary>Group navigation property.</summary>
    public virtual Groups.Group Group { get; private set; }

    /// <summary>Identifier of the competition context.</summary>
    public Guid CompetitionId { get; private set; }

    /// <summary>Competition navigation property.</summary>
    public virtual Competitions.Competition Competition { get; private set; }

    /// <summary>Source code submitted.</summary>
    public string Code { get; private set; }

    /// <summary>Execution time reported by the judge.</summary>
    public TimeSpan Time { get; private set; }

    /// <summary>Submission timestamp (UTC).</summary>
    public DateTime SubmissionTime { get; private set; }

    /// <summary>Programming language used.</summary>
    public LanguageType Language { get; private set; }

    /// <summary>Whether the submission was accepted.</summary>
    public bool Accepted { get; private set; }

    /// <summary>Judge response status.</summary>
    public JudgeSubmissionResponse JudgeResponse { get; private set; }

#pragma warning disable CS8618
    protected GroupExerciseAttempt() { }
#pragma warning restore CS8618

    /// <summary>Creates a new group attempt with the provided metadata.</summary>
    public GroupExerciseAttempt(
        Exercise exercise,
        Groups.Group group,
        Competitions.Competition competition,
        string code,
        LanguageType language
    )
    {
        if (exercise == null)
            throw new ArgumentNullException(nameof(exercise));
        if (group == null)
            throw new ArgumentNullException(nameof(group));
        if (competition == null)
            throw new ArgumentNullException(nameof(competition));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("O código não pode estar vazio");

        Exercise = exercise;
        ExerciseId = exercise.Id;
        Group = group;
        GroupId = group.Id;
        Competition = competition;
        CompetitionId = competition.Id;
        Code = code;
        Language = language;
        SubmissionTime = DateTime.UtcNow;
        Time = TimeSpan.Zero;
        Accepted = false;
        JudgeResponse = JudgeSubmissionResponse.Pending;
    }

    /// <summary>Sets the judge's response and execution time for this attempt.</summary>
    /// <param name="response">Judge status.</param>
    /// <param name="executionTime">Execution duration.</param>
    public void SetJudgeResponse(JudgeSubmissionResponse response, TimeSpan executionTime)
    {
        JudgeResponse = response;
        Time = executionTime;
        Accepted = response == JudgeSubmissionResponse.Accepted;
    }
}
