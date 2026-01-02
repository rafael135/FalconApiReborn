namespace Falcon.Core.Domain.Exercises;

public class ExerciseType
{
    public int Id { get; private set; }
    public string Label { get; private set; }

    private readonly List<Exercise> _exercises = new();
    public virtual IReadOnlyCollection<Exercise> Exercises => _exercises.AsReadOnly();

#pragma warning disable CS8618
    protected ExerciseType() { }
#pragma warning restore CS8618

    public ExerciseType(int id, string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Rótulo é obrigatório");
        }

        Id = id;
        Label = label;
    }
}
