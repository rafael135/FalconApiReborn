using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Groups;

/// <summary>
/// Represents an invitation sent to a user to join a group.
/// </summary>
public class GroupInvite : Entity
{
    /// <summary>
    /// The group to which the user is invited.
    /// </summary>
    public Guid GroupId { get; private set; }

    /// <summary>
    /// The user who is invited to join the group.
    /// </summary>
    public virtual Group Group { get; private set; }

    /// <summary>
    /// The user ID of the invited user.
    /// </summary>
    public string UserId { get; private set; }

    /// <summary>
    /// The invited user.
    /// </summary>
    public virtual User User { get; private set; }

    /// <summary>
    /// Indicates whether the invite has been accepted.
    /// </summary>
    public bool Accepted { get; private set; }

    /// <summary>
    /// Required by EF Core.
    /// </summary>
#pragma warning disable CS8618
    protected GroupInvite() { }
#pragma warning restore CS8618

    /// <summary>
    /// Creates a new group invite for the specified user and group.
    /// </summary>
    /// <param name="group">The group to which the user is invited.</param>
    /// <param name="user">The user who is invited to join the group.</param>
    /// <exception cref="ArgumentNullException">If either the group or user is null.</exception>
    public GroupInvite(Group group, User user)
    {
        if (group == null)
            throw new ArgumentNullException(nameof(group));
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        Group = group;
        GroupId = group.Id;
        User = user;
        UserId = user.Id;
        Accepted = false;
    }

    /// <summary>
    /// Marks the invite as accepted.
    /// </summary>
    public void MarkAsAccepted()
    {
        Accepted = true;
    }
}
