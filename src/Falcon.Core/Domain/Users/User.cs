using Falcon.Core.Domain.Groups;
using Microsoft.AspNetCore.Identity;

namespace Falcon.Core.Domain.Users;

/// <summary>
/// Represents an application user with profile and group membership information.
/// Inherits from <see cref="IdentityUser"/> to integrate with ASP.NET Identity.
/// </summary>
public class User : IdentityUser
{
    /// <summary>
    /// The full name of the user.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The RA (student registration number) of the user.
    /// </summary>
    public string RA { get; private set; }

    /// <summary>
    /// Optional year the user joined the institution.
    /// </summary>
    public int? JoinYear { get; private set; }

    /// <summary>
    /// Optional academic department of the user.
    /// </summary>
    public string? Department { get; private set; }

    /// <summary>
    /// Date/time when the user was created (UTC).
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date/time when the user last logged in (UTC).
    /// </summary>
    public DateTime LastLoggedAt { get; private set; }

    /// <summary>
    /// Soft-delete flag.
    /// </summary>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Date/time when the user was soft-deleted (UTC), if applicable.
    /// </summary>
    public DateTime? DeletedAt { get; private set; }

    /// <summary>
    /// The current group identifier the user belongs to, if any.
    /// </summary>
    public Guid? GroupId { get; private set; }

    /// <summary>
    /// Navigation property to the user's group.
    /// </summary>
    public virtual Group? Group { get; private set; }

    private readonly List<Exercises.Question> _questions = new();

    /// <summary>
    /// Questions authored by the user.
    /// </summary>
    public virtual IReadOnlyCollection<Exercises.Question> Questions => _questions.AsReadOnly();

    private readonly List<Exercises.Answer> _answers = new();

    /// <summary>
    /// Answers authored by the user.
    /// </summary>
    public virtual IReadOnlyCollection<Exercises.Answer> Answers => _answers.AsReadOnly();

    private readonly List<Auditing.Log> _logs = new();

    /// <summary>
    /// Audit logs related to the user.
    /// </summary>
    public virtual IReadOnlyCollection<Auditing.Log> Logs => _logs.AsReadOnly();

    /// <summary>
    /// Parameterless constructor required by EF Core.
    /// </summary>
    public User() { }

    /// <summary>
    /// Initializes a new user with the provided profile details.
    /// </summary>
    /// <param name="name">The user's full name.</param>
    /// <param name="email">The user's email address (also used as username).</param>
    /// <param name="ra">The user's RA (student registration number).</param>
    /// <param name="joinYear">Optional join year.</param>
    /// <param name="department">Optional department name.</param>
    /// <exception cref="ArgumentException">Thrown when required fields are missing or invalid.</exception>
    public User(
        string name,
        string email,
        string ra,
        int? joinYear = null,
        string? department = null
    )
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome inválido");
        if (string.IsNullOrWhiteSpace(ra))
            throw new ArgumentException("RA inválido");

        Name = name;
        Email = email;
        UserName = email; // Identity requirement
        RA = ra;
        JoinYear = joinYear;
        Department = department;
        CreatedAt = DateTime.UtcNow;

        // Initialize identity flags
        EmailConfirmed = false;
        PhoneNumberConfirmed = false;
        TwoFactorEnabled = false;
    }

    /// <summary>
    /// Updates the user's last login timestamp to the current UTC time.
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoggedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Assigns the user to a group.
    /// </summary>
    /// <param name="group">The group to assign the user to.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="group"/> is null.</exception>
    public void AssignGroup(Group group)
    {
        if (group == null) throw new ArgumentNullException(nameof(group));
        Group = group;
        GroupId = group.Id;
    }

    /// <summary>
    /// Removes the user's association with any group.
    /// </summary>
    public void LeaveGroup()
    {
        Group = null;
        GroupId = null;
    }

    /// <summary>
    /// Updates the user's profile fields.
    /// </summary>
    /// <param name="name">New name.</param>
    /// <param name="ra">New RA.</param>
    /// <param name="joinYear">New join year.</param>
    /// <param name="department">New department.</param>
    /// <exception cref="ArgumentException">Thrown when required fields are invalid.</exception>
    public void UpdateProfile(string name, string ra, int? joinYear, string? department)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome inválido");
        if (string.IsNullOrWhiteSpace(ra))
            throw new ArgumentException("RA inválido");

        Name = name;
        RA = ra;
        JoinYear = joinYear;
        Department = department;
    }

    /// <summary>
    /// Soft-deletes the user by setting <see cref="IsDeleted"/> and <see cref="DeletedAt"/>.
    /// </summary>
    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }
}
