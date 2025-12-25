namespace Falcon.Core.Domain.Exercises;

public class ExerciseOutput : Entity
{
    public Guid ExerciseId { get; private set; }
    public virtual Exercise Exercise { get; private set; }
    public string OutputContent { get; private set; }
    public string? JudgeUuid { get; private set; }
    public Guid ExerciseInputId { get; private set; }

    protected ExerciseOutput() {}

    public ExerciseOutput(string outputContent, Exercise exercise)
    {
        if (string.IsNullOrWhiteSpace(outputContent))
        {
            throw new ArgumentException("Output content is required");
        }

        OutputContent = outputContent;
        Exercise = exercise;
        ExerciseId = exercise.Id;
    }
}