namespace Falcon.Core.Domain.Exercises;

public class ExerciseOutput : Entity
{
    public Guid ExerciseId { get; private set; }
    public virtual Exercise Exercise { get; private set; }
    public string OutputContent { get; private set; }
    public string? JudgeUuid { get; private set; }
    public Guid ExerciseInputId { get; private set; }
    public virtual ExerciseInput? ExerciseInput { get; private set; }

#pragma warning disable CS8618
    protected ExerciseOutput() {}
#pragma warning restore CS8618

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