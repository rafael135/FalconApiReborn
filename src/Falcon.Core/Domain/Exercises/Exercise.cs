namespace Falcon.Core.Domain.Exercises;

public class Exercise : Entity
{
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public TimeSpan EstimatedTime { get; private set; }
    public string? JudgeUuid { get; private set; }
    public int ExerciseTypeId { get; private set; }
    public ExerciseType ExerciseType { get; private set; }

    public Guid? AttachedFileId { get; private set; }
    public virtual Files.AttachedFile? AttachedFile { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private readonly List<ExerciseInput> _inputs = new();
    public virtual IReadOnlyCollection<ExerciseInput> Inputs => _inputs.AsReadOnly();

    private readonly List<ExerciseOutput> _outputs = new();
    public virtual IReadOnlyCollection<ExerciseOutput> Outputs => _outputs.AsReadOnly();

    private readonly List<Question> _questions = new();
    public virtual IReadOnlyCollection<Question> Questions => _questions.AsReadOnly();

    private readonly List<GroupExerciseAttempt> _attempts = new();
    public virtual IReadOnlyCollection<GroupExerciseAttempt> Attempts => _attempts.AsReadOnly();

    protected Exercise() { }

    public Exercise(string title, string? description, int exerciseTypeId, TimeSpan estimatedTime)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required");
        }

        Title = title;
        Description = description;
        ExerciseTypeId = exerciseTypeId;
        EstimatedTime = estimatedTime;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddTestCase(string inputContent, string expectedOutput)
    {
        var input = new ExerciseInput(inputContent);
        var output = new ExerciseOutput(expectedOutput, this);

        _inputs.Add(input);
        _outputs.Add(output);
    }

    public void ClearTestCases()
    {
        _inputs.Clear();
        _outputs.Clear();
    }

    public void SetJudgeUuid(string judgeUuid)
    {
        if (string.IsNullOrWhiteSpace(judgeUuid))
        {
            throw new ArgumentException("Invalid UUID");
        }

        JudgeUuid = judgeUuid;
    }

    public void UpdateDetails(string title, string? description, int exerciseTypeId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required");
        }

        Title = title;
        Description = description;
        ExerciseTypeId = exerciseTypeId;
    }

    public void SetAttachedFile(Files.AttachedFile file)
    {
        AttachedFile = file;
        AttachedFileId = file.Id;
    }
}
