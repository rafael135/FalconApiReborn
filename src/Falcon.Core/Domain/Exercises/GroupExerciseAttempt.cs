using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Core.Domain.Exercises;

/// <summary>
/// Represents an attempt by a group to solve an exercise within a competition.
/// </summary>
public class GroupExerciseAttempt : Entity
{
    public Guid ExerciseId { get; private set; }
    public virtual Exercise Exercise { get; private set; }

    public Guid GroupId { get; private set; }
    public virtual Groups.Group Group { get; private set; }

    public Guid CompetitionId { get; private set; }
    public virtual Competitions.Competition Competition { get; private set; }

    public string Code { get; private set; }
    public TimeSpan Time { get; private set; }
    public DateTime SubmissionTime { get; private set; }
    public LanguageType Language { get; private set; }
    public bool Accepted { get; private set; }
    public JudgeSubmissionResponse JudgeResponse { get; private set; }

#pragma warning disable CS8618
    protected GroupExerciseAttempt() { }
#pragma warning restore CS8618

    public GroupExerciseAttempt(
        Exercise exercise,
        Groups.Group group,
        Competitions.Competition competition,
        string code,
        LanguageType language)
    {
        if (exercise == null)
            throw new ArgumentNullException(nameof(exercise));
        if (group == null)
            throw new ArgumentNullException(nameof(group));
        if (competition == null)
            throw new ArgumentNullException(nameof(competition));
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Code cannot be empty");

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

    public void SetJudgeResponse(JudgeSubmissionResponse response, TimeSpan executionTime)
    {
        JudgeResponse = response;
        Time = executionTime;
        Accepted = response == JudgeSubmissionResponse.Accepted;
    }
}
