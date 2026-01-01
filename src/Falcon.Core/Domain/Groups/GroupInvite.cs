using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Groups;

public class GroupInvite : Entity
{
    public Guid GroupId { get; private set; }
    public virtual Group Group { get; private set; }
    public string UserId { get; private set; }
    public virtual User User { get; private set; }

    public bool Accepted { get; private set; }

#pragma warning disable CS8618
    protected GroupInvite() { }
#pragma warning restore CS8618

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

    public void MarkAsAccepted()
    {
        Accepted = true;
    }
}
