namespace Falcon.Core.Domain.Exercises;

public class ExerciseType
{
    public int Id { get; private set; }
    public string Label { get; private set; }

    protected ExerciseType() { }

    public ExerciseType(int id, string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Label is required");
        }

        Id = id;
        Label = label;
    }
}
