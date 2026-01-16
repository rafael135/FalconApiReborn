namespace Falcon.Core.Domain.Files;

/// <summary>
/// Represents a file attached to domain entities, with metadata and storage path information.
/// </summary>
public class AttachedFile : Entity
{
    /// <summary>
    /// Original filename as uploaded.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// MIME type of the file.
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long Size { get; private set; }

    /// <summary>
    /// Relative file path in the storage provider.
    /// </summary>
    public string FilePath { get; private set; }

    /// <summary>
    /// Date/time when file metadata was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    private readonly List<Exercises.Exercise> _exercises = new();

    /// <summary>
    /// Exercises that reference this attached file.
    /// </summary>
    public virtual IReadOnlyCollection<Exercises.Exercise> Exercises => _exercises.AsReadOnly();

#pragma warning disable CS8618
    protected AttachedFile() { }
#pragma warning restore CS8618

    /// <summary>
    /// Creates a new attached file metadata instance.
    /// </summary>
    /// <param name="name">Original filename.</param>
    /// <param name="type">MIME type.</param>
    /// <param name="size">File size in bytes.</param>
    /// <param name="filePath">Relative storage path.</param>
    /// <exception cref="ArgumentException">Thrown when required values are missing or invalid.</exception>
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

    /// <summary>
    /// Associates this attached file with an <see cref="Exercises.Exercise"/> instance.
    /// </summary>
    /// <param name="exercise">The exercise to attach to.</param>
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
