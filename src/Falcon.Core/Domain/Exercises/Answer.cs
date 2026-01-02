using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Exercises;

/// <summary>
/// Represents an answer to a question in the system.
/// </summary>
public class Answer : Entity
{
    public string Content { get; private set; }
    public string UserId { get; private set; }
    public virtual User User { get; private set; }
    public virtual Question? Question { get; private set; }
    public DateTime CreatedAt { get; private set; }

#pragma warning disable CS8618
    protected Answer() { }
#pragma warning restore CS8618

    public Answer(User user, string content)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("O conteúdo da resposta não pode estar vazio");

        User = user;
        UserId = user.Id;
        Content = content;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateContent(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("O conteúdo da resposta não pode estar vazio");

        Content = content;
    }
}
