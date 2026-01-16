namespace Falcon.Core.Domain.Exercises;

/// <summary>
/// Represents an input (test case input) for an exercise.
/// </summary>
public class ExerciseInput : Entity
{
    /// <summary>The input content string.</summary>
    public string InputContent { get; private set; }

    /// <summary>Optional Judge UUID associated with this input.</summary>
    public string? JudgeUuid { get; private set; }

    /// <summary>Foreign key to the parent exercise.</summary>
    public Guid ExerciseId { get; private set; }

    /// <summary>Optional linked expected output.</summary>
    public virtual ExerciseOutput? Output { get; private set; }

#pragma warning disable CS8618
    protected ExerciseInput() { }
#pragma warning restore CS8618

    /// <summary>Creates a new input with the specified content.</summary>
    /// <param name="inputContent">The input content.</param>
    public ExerciseInput(string inputContent)
    {
        if (string.IsNullOrWhiteSpace(inputContent))
        {
            throw new ArgumentException("Conteúdo da entrada é obrigatório");
        }

        InputContent = inputContent;
    }

    /// <summary>Associates this input with an exercise.</summary>
    /// <param name="exercise">The parent exercise.</param>
    public void SetExercise(Exercise exercise)
    {
        if (exercise == null)
        {
            throw new ArgumentNullException(nameof(exercise));
        }

        ExerciseId = exercise.Id;
    }
}
