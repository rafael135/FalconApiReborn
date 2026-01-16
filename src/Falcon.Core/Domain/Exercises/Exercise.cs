using Falcon.Core.Domain.Exercises.Rules;

namespace Falcon.Core.Domain.Exercises;

/// <summary>
/// Represents a programming exercise with inputs, outputs, and metadata used by competitions and the judge.
/// </summary>
public class Exercise : Entity
{
    /// <summary>The exercise title.</summary>
    public string Title { get; private set; }

    /// <summary>The exercise description (optional).</summary>
    public string? Description { get; private set; }

    /// <summary>Estimated time to solve the exercise.</summary>
    public TimeSpan EstimatedTime { get; private set; }

    /// <summary>Optional UUID of the exercise in the external Judge system.</summary>
    public string? JudgeUuid { get; private set; }

    /// <summary>Foreign key to the exercise type.</summary>
    public int ExerciseTypeId { get; private set; }

    /// <summary>Navigation to the exercise type.</summary>
    public ExerciseType ExerciseType { get; private set; }

    /// <summary>Attached file id (if any).</summary>
    public Guid? AttachedFileId { get; private set; }

    /// <summary>Attached file navigation property.</summary>
    public virtual Files.AttachedFile? AttachedFile { get; private set; }

    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; private set; }

    private readonly List<ExerciseInput> _inputs = new();

    /// <summary>Collection of exercise inputs (test case inputs).</summary>
    public virtual IReadOnlyCollection<ExerciseInput> Inputs => _inputs.AsReadOnly();

    private readonly List<ExerciseOutput> _outputs = new();

    /// <summary>Collection of expected outputs for test cases.</summary>
    public virtual IReadOnlyCollection<ExerciseOutput> Outputs => _outputs.AsReadOnly();

    private readonly List<Question> _questions = new();

    /// <summary>Questions related to this exercise.</summary>
    public virtual IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    private readonly List<GroupExerciseAttempt> _attempts = new();

    /// <summary>Attempts made by groups on this exercise.</summary>
    public virtual IReadOnlyCollection<GroupExerciseAttempt> Attempts => _attempts.AsReadOnly();

    /// <summary>Parameterless constructor for EF Core.</summary>
    protected Exercise() { }

    /// <summary>Constructs a new exercise instance.</summary>
    /// <param name="title">The title.</param>
    /// <param name="description">Optional description.</param>
    /// <param name="exerciseTypeId">Exercise type identifier.</param>
    /// <param name="estimatedTime">Estimated solving time.</param>
    /// <exception cref="ArgumentException">Thrown when required fields are invalid.</exception>
    public Exercise(string title, string? description, int exerciseTypeId, TimeSpan estimatedTime)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Título é obrigatório");
        }

        Title = title;
        Description = description;
        ExerciseTypeId = exerciseTypeId;
        EstimatedTime = estimatedTime;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a test case (input and expected output) to the exercise.
    /// </summary>
    /// <param name="inputContent">Test input content.</param>
    /// <param name="expectedOutput">Expected output content.</param>
    public void AddTestCase(string inputContent, string expectedOutput)
    {
        var input = new ExerciseInput(inputContent);
        var output = new ExerciseOutput(expectedOutput, this);

        _inputs.Add(input);
        _outputs.Add(output);
    }

    /// <summary>
    /// Removes all test cases from the exercise.
    /// </summary>
    public void ClearTestCases()
    {
        _inputs.Clear();
        _outputs.Clear();
    }

    /// <summary>
    /// Sets the external Judge UUID for this exercise, validating format using domain rule.
    /// </summary>
    /// <param name="judgeUuid">The Judge UUID string.</param>
    public void SetJudgeUuid(string judgeUuid)
    {
        CheckRule(new ExerciseMustHaveValidUuidRule(judgeUuid));

        JudgeUuid = judgeUuid;
    }

    /// <summary>
    /// Updates core exercise details.
    /// </summary>
    /// <param name="title">The new title.</param>
    /// <param name="description">The new description.</param>
    /// <param name="exerciseTypeId">New exercise type id.</param>
    public void UpdateDetails(string title, string? description, int exerciseTypeId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Título é obrigatório");
        }

        Title = title;
        Description = description;
        ExerciseTypeId = exerciseTypeId;
    }

    /// <summary>
    /// Attaches a file to the exercise.
    /// </summary>
    /// <param name="file">The attached file.</param>
    public void SetAttachedFile(Files.AttachedFile file)
    {
        AttachedFile = file;
        AttachedFileId = file.Id;
    }
}
