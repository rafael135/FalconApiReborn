using Falcon.Core.Domain.Groups;
using Microsoft.AspNetCore.Identity;

namespace Falcon.Core.Domain.Users;

public class User : IdentityUser
{
    public string Name { get; private set; }
    public string RA { get; private set; }
    public int? JoinYear { get; private set; }
    public string? Department { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime LastLoggedAt { get; private set; }

    public Guid? GroupId { get; private set; }
    public virtual Group? Group { get; private set; }

    private readonly List<Exercises.Question> _questions = new();
    public virtual IReadOnlyCollection<Exercises.Question> Questions => _questions.AsReadOnly();

    private readonly List<Exercises.Answer> _answers = new();
    public virtual IReadOnlyCollection<Exercises.Answer> Answers => _answers.AsReadOnly();

    private readonly List<Auditing.Log> _logs = new();
    public virtual IReadOnlyCollection<Auditing.Log> Logs => _logs.AsReadOnly();

    public User() { }

    public User(
        string name,
        string email,
        string ra,
        int? joinYear = null,
        string? department = null
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name invalid");
        if (string.IsNullOrWhiteSpace(ra))
            throw new ArgumentException("RA invalid");

        Name = name;
        Email = email;
        UserName = email; // Identity exige isso
        RA = ra;
        JoinYear = joinYear;
        Department = department;
        CreatedAt = DateTime.UtcNow;

        // Inicializa flags do Identity
        EmailConfirmed = false;
        PhoneNumberConfirmed = false;
        TwoFactorEnabled = false;
    }

    public void UpdateLastLogin()
    {
        LastLoggedAt = DateTime.UtcNow;
    }

    public void AssignGroup(Group group)
    {
        Group = group;
        GroupId = group.Id;
    }
}
