using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Groups;

public class GroupInvite : Entity
{
    public Guid GroupId { get; private set; }
    public virtual Group Group { get; private set; }
    public string UserId { get; private set; }
    public virtual User User { get; private set; }

    public bool Accepted { get; private set; }

    protected GroupInvite() { }

    public GroupInvite(Group group, string inviteCode, DateTime expirationDate)
    {
        if (group == null)
        {
            throw new ArgumentNullException(nameof(group));
        }

        if (string.IsNullOrWhiteSpace(inviteCode))
        {
            throw new ArgumentException("Invite code is required");
        }

        Group = group;
        GroupId = group.Id;
        Accepted = false;
    }

    public void MarkAsAccepted()
    {
        Accepted = true;
    }
}
