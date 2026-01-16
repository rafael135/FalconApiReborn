using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Exercises;

/// <summary>
/// Represents an answer to a <see cref="Question"/>, authored by a user.
/// </summary>
public class Answer : Entity
{
    /// <summary>Maximum content length for the answer.</summary>
    public const int MaxContentLength = 2000;

    /// <summary>The textual content of the answer.</summary>
    public string Content { get; private set; }

    /// <summary>Author user identifier.</summary>
    public string UserId { get; private set; }

    /// <summary>Author user navigation property.</summary>
    public virtual User User { get; private set; }

    /// <summary>Optional related question navigation property.</summary>
    public virtual Question? Question { get; private set; }

    /// <summary>Creation timestamp (UTC).</summary>
    public DateTime CreatedAt { get; private set; }

#pragma warning disable CS8618
    protected Answer() { }
#pragma warning restore CS8618

    /// <summary>Creates a new answer authored by <paramref name="user"/> with the provided content.</summary>
    public Answer(User user, string content)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("O conteúdo da resposta não pode estar vazio");
        if (content.Length > MaxContentLength)
            throw new ArgumentException($"Conteúdo não pode exceder {MaxContentLength} caracteres");

        User = user;
        UserId = user.Id;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>Updates the content of the answer.</summary>
    /// <param name="content">New content.</param>
    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("O conteúdo da resposta não pode estar vazio");
        if (content.Length > MaxContentLength)
            throw new ArgumentException($"Conteúdo não pode exceder {MaxContentLength} caracteres");

        Content = content;
    }
}
