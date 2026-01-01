using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Shared.Exceptions;

namespace Falcon.Core.Domain.Competitions;

public class Competition : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public CompetitionStatus Status { get; private set; }

    public int? MaxMembers { get; private set; }
    public int? MaxExercises { get; private set; }
    public int? MaxSubmissionSize { get; private set; }
    public TimeSpan? SubmissionPenalty { get; private set; }

    public DateTime StartInscriptions { get; private set; }
    public DateTime EndInscriptions { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public TimeSpan? Duration { get; private set; }

    public DateTime? StopRanking { get; private set; }
    public DateTime? BlockSubmissions { get; private set; }

    public bool IsInscriptionOpen =>
        DateTime.UtcNow >= StartInscriptions && DateTime.UtcNow < EndInscriptions;

    private readonly List<ExerciseInCompetition> _exercisesInCompetition = new();
    public virtual IReadOnlyCollection<ExerciseInCompetition> ExercisesInCompetition => _exercisesInCompetition.AsReadOnly();

    private readonly List<GroupInCompetition> _groupsInCompetitions = new();
    public virtual IReadOnlyCollection<GroupInCompetition> GroupsInCompetitions => _groupsInCompetitions.AsReadOnly();

    private readonly List<CompetitionRanking> _rankings = new();
    public virtual IReadOnlyCollection<CompetitionRanking> Rankings => _rankings.AsReadOnly();

    private readonly List<Exercises.GroupExerciseAttempt> _attempts = new();
    public virtual IReadOnlyCollection<Exercises.GroupExerciseAttempt> Attempts => _attempts.AsReadOnly();

    private readonly List<Exercises.Question> _questions = new();
    public virtual IReadOnlyCollection<Exercises.Question> Questions => _questions.AsReadOnly();

    private readonly List<Auditing.Log> _logs = new();
    public virtual IReadOnlyCollection<Auditing.Log> Logs => _logs.AsReadOnly();

#pragma warning disable CS8618
    protected Competition() { }
#pragma warning restore CS8618

    public static Competition CreateTemplate(
        string name,
        string description,
        DateTime startInscriptions,
        DateTime endInscriptions,
        DateTime startTime
    )
    {
        if (endInscriptions < startInscriptions)
        {
            throw new DomainException("End of inscriptions cannot be before start of inscriptions");
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
        if (Status != CompetitionStatus.ModelTemplate)
        {
            throw new DomainException(
                "Only template competitions can be promoted to active competitions"
            );
        }

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
    public void OpenInscriptions()
    {
        if (Status != CompetitionStatus.Pending)
            throw new DomainException("Competition not ready.");
        Status = CompetitionStatus.OpenInscriptions;
    }

    public void Start()
    {
        Status = CompetitionStatus.Ongoing;
    }

    public void Finish()
    {
        Status = CompetitionStatus.Finished;
    }

    public void AddExercise(Exercise exercise)
    {
        if (ExercisesInCompetition.Any(e => e.ExerciseId == exercise.Id))
        {
            throw new DomainException("Exercise already added to competition.");
        }

        ExerciseInCompetition exerciseInCompetition = new ExerciseInCompetition(this, exercise);
        _exercisesInCompetition.Add(exerciseInCompetition);
    }
}
