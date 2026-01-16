namespace Falcon.Core.Domain.Exercises;

/// <summary>
/// Represents the expected output for a specific exercise input (test case output).
/// </summary>
public class ExerciseOutput : Entity
{
    /// <summary>Foreign key to the parent exercise.</summary>
    public Guid ExerciseId { get; private set; }

    /// <summary>Navigation to the parent exercise.</summary>
    public virtual Exercise Exercise { get; private set; }

    /// <summary>The expected output content for a test case.</summary>
    public string OutputContent { get; private set; }

    /// <summary>Optional Judge UUID for this output.</summary>
    public string? JudgeUuid { get; private set; }

    /// <summary>Foreign key to the corresponding input, if linked.</summary>
    public Guid ExerciseInputId { get; private set; }

    /// <summary>Navigation to the corresponding input.</summary>
    public virtual ExerciseInput? ExerciseInput { get; private set; }

#pragma warning disable CS8618
    protected ExerciseOutput() { }
#pragma warning restore CS8618

    /// <summary>Creates a new expected output and links it to the provided exercise.</summary>
    /// <param name="outputContent">The expected output content.</param>
    /// <param name="exercise">The parent exercise.</param>
    public ExerciseOutput(string outputContent, Exercise exercise)
    {
        if (string.IsNullOrWhiteSpace(outputContent))
        {
            throw new ArgumentException("Conteúdo da saída é obrigatório");
        }

        if (exercise == null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }

        OutputContent = outputContent;
        Exercise = exercise;
        ExerciseId = exercise.Id;
    }
}
