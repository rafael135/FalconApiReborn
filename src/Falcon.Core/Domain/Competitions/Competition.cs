using Falcon.Core.Domain.Competitions.Rules;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Shared.Exceptions;

namespace Falcon.Core.Domain.Competitions;

/// <summary>
/// Represents a competition template or active competition containing exercises, enrollments and rankings.
/// </summary>
public class Competition : Entity
{
    /// <summary>Competition name.</summary>
    public string Name { get; private set; }

    /// <summary>Competition description.</summary>
    public string Description { get; private set; }

    /// <summary>Current competition status.</summary>
    public CompetitionStatus Status { get; private set; }

    /// <summary>Maximum members allowed per group (if set).</summary>
    public int? MaxMembers { get; private set; }

    /// <summary>Maximum exercises allowed in the competition (if set).</summary>
    public int? MaxExercises { get; private set; }

    /// <summary>Maximum submission size in bytes (if set).</summary>
    public int? MaxSubmissionSize { get; private set; }

    /// <summary>Penalty applied to late submissions (if set).</summary>
    public TimeSpan? SubmissionPenalty { get; private set; }

    /// <summary>Enrollment start time (UTC).</summary>
    public DateTime StartInscriptions { get; private set; }

    /// <summary>Enrollment end time (UTC).</summary>
    public DateTime EndInscriptions { get; private set; }

    /// <summary>Competition start time (UTC).</summary>
    public DateTime StartTime { get; private set; }

    /// <summary>Competition end time (UTC), if set.</summary>
    public DateTime? EndTime { get; private set; }

    /// <summary>Competition duration, if set.</summary>
    public TimeSpan? Duration { get; private set; }

    /// <summary>Timestamp after which ranking calculation stops, if set.</summary>
    public DateTime? StopRanking { get; private set; }

    /// <summary>Timestamp after which submissions are blocked, if set.</summary>
    public DateTime? BlockSubmissions { get; private set; }

    /// <summary>Whether inscriptions are currently open.</summary>
    public bool IsInscriptionOpen =>
        DateTime.UtcNow >= StartInscriptions && DateTime.UtcNow < EndInscriptions;

    private readonly List<ExerciseInCompetition> _exercisesInCompetition = new();
    public virtual IReadOnlyCollection<ExerciseInCompetition> ExercisesInCompetition =>
        _exercisesInCompetition.AsReadOnly();

    private readonly List<GroupInCompetition> _groupsInCompetitions = new();
    public virtual IReadOnlyCollection<GroupInCompetition> GroupsInCompetitions =>
        _groupsInCompetitions.AsReadOnly();

    private readonly List<CompetitionRanking> _rankings = new();
    public virtual IReadOnlyCollection<CompetitionRanking> Rankings => _rankings.AsReadOnly();

    private readonly List<Exercises.GroupExerciseAttempt> _attempts = new();
    public virtual IReadOnlyCollection<Exercises.GroupExerciseAttempt> Attempts =>
        _attempts.AsReadOnly();

    private readonly List<Exercises.Question> _questions = new();
    public virtual IReadOnlyCollection<Exercises.Question> Questions => _questions.AsReadOnly();

    private readonly List<Auditing.Log> _logs = new();
    public virtual IReadOnlyCollection<Auditing.Log> Logs => _logs.AsReadOnly();

#pragma warning disable CS8618
    protected Competition() { }
#pragma warning restore CS8618

    /// <summary>
    /// Creates a competition template used as a starting point for promotion into a runnable competition.
    /// </summary>
    public static Competition CreateTemplate(
        string name,
        string description,
        DateTime startInscriptions,
        DateTime endInscriptions,
        DateTime startTime
    )
    {
        // Validate business rule before creating instance
        if (
            new EndInscriptionsCannotBeBeforeStartRule(
                startInscriptions,
                endInscriptions
            ).IsBroken()
        )
        {
            throw new BusinessRuleException(
                new EndInscriptionsCannotBeBeforeStartRule(startInscriptions, endInscriptions)
            );
        }

        return new Competition
        {
            Name = name,
            Description = description,
            StartInscriptions = startInscriptions,
            EndInscriptions = endInscriptions,
            StartTime = startTime,
            Status = CompetitionStatus.ModelTemplate,

            MaxMembers = 3,
            MaxExercises = 3,
            Duration = TimeSpan.FromHours(2),
            SubmissionPenalty = TimeSpan.FromMinutes(10),
        };
    }

    /// <summary>
    /// Promotes a template into a configured competition with limits and time windows.
    /// </summary>
    public void PromoteToCompetition(
        int maxMembers,
        int maxExercises,
        int maxSubmissionSize,
        TimeSpan duration,
        TimeSpan stopRanking,
        TimeSpan blockSubmissions,
        TimeSpan penalty
    )
    {
        CheckRule(new OnlyTemplateCanBePromotedRule(Status));

        MaxMembers = maxMembers;
        MaxExercises = maxExercises;
        MaxSubmissionSize = maxSubmissionSize;
        SubmissionPenalty = penalty;

        EndTime = StartTime.Add(duration);
        StopRanking = StartTime.Add(stopRanking);
        BlockSubmissions = StartTime.Add(blockSubmissions);

        // If it was como Template, vira Pending (pronto para ser ativado pelo Worker ou Admin)
        if (Status == CompetitionStatus.ModelTemplate)
        {
            Status = CompetitionStatus.Pending;
        }
    }

    // Comportamentos de Ciclo de Vida
    /// <summary>
    /// Opens inscriptions for the competition (transitions to OpenInscriptions status).
    /// </summary>
    public void OpenInscriptions()
    {
        CheckRule(new CompetitionMustBePendingToOpenInscriptionsRule(Status));
        Status = CompetitionStatus.OpenInscriptions;
    }

    /// <summary>
    /// Starts the competition (status -> Ongoing).
    /// </summary>
    public void Start()
    {
        Status = CompetitionStatus.Ongoing;
    }

    /// <summary>
    /// Marks the competition as finished.
    /// </summary>
    public void Finish()
    {
        Status = CompetitionStatus.Finished;
    }

    /// <summary>
    /// Adds an exercise to the competition, enforcing domain rules.
    /// </summary>
    /// <param name="exercise">The exercise to add.</param>
    public void AddExercise(Exercise exercise)
    {
        bool isAlreadyAdded = ExercisesInCompetition.Any(e => e.ExerciseId == exercise.Id);
        CheckRule(new ExerciseCannotBeAddedTwiceRule(isAlreadyAdded));

        ExerciseInCompetition exerciseInCompetition = new ExerciseInCompetition(this, exercise);
        _exercisesInCompetition.Add(exerciseInCompetition);
    }
}
