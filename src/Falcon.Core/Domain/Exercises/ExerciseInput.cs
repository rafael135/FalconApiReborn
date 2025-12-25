namespace Falcon.Core.Domain.Exercises;

public class ExerciseInput : Entity
{
    public string InputContent { get; private set; }
    public string? JudgeUuid { get; private set; }

    public Guid ExerciseId { get; private set; }

    public virtual ExerciseOutput Output { get; private set; }

    protected ExerciseInput() { }

    public ExerciseInput(string inputContent)
    {
        if (string.IsNullOrWhiteSpace(inputContent))
        {
            throw new ArgumentException("Input content is required");
        }

        InputContent = inputContent;
    }

    public void SetExercise(Exercise exercise)
    {
        if (exercise == null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }

        ExerciseId = exercise.Id;
    }
}
