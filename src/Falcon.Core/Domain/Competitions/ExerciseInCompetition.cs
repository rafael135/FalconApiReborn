using Falcon.Core.Domain.Exercises;

namespace Falcon.Core.Domain.Competitions;

public class ExerciseInCompetition
{
    public Guid CompetitionId { get; private set; }
    public Guid ExerciseId { get; private set; }

    public virtual Competition? Competition { get; private set; } = null;
    public virtual Exercise? Exercise { get; private set; } = null;

    protected ExerciseInCompetition() { }

    public ExerciseInCompetition(Competition competition, Exercise exercise)
    {
        if (competition == null)
        {
            throw new ArgumentNullException(nameof(competition));
        }

        if (exercise == null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }

        Competition = competition;
        CompetitionId = competition.Id;
        Exercise = exercise;
        ExerciseId = exercise.Id;
    }
}
