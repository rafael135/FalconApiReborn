namespace Falcon.Core.Domain.Files;

/// <summary>
/// Represents a file that is attached to an entity, including its metadata.
/// </summary>
public class AttachedFile : Entity
{
    public string Name { get; private set; }
    public string Type { get; private set; }
    public long Size { get; private set; }
    public string FilePath { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<Exercises.Exercise> _exercises = new();
    public virtual IReadOnlyCollection<Exercises.Exercise> Exercises => _exercises.AsReadOnly();

#pragma warning disable CS8618
    protected AttachedFile() { }
#pragma warning restore CS8618

    public AttachedFile(string name, string type, long size, string filePath)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do arquivo é obrigatório");
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Tipo do arquivo é obrigatório");
        if (size <= 0)
            throw new ArgumentException("Tamanho do arquivo deve ser maior que 0");
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Caminho do arquivo é obrigatório");

        Name = name;
        Type = type;
        Size = size;
        FilePath = filePath;
        CreatedAt = DateTime.UtcNow;
    }

    public void AttachToExercise(Exercises.Exercise exercise)
    {
        if (exercise == null)
            throw new ArgumentNullException(nameof(exercise));

        if (!_exercises.Any(e => e.Id == exercise.Id))
        {
            _exercises.Add(exercise);
        }
    }
}
