using Falcon.Core.Domain.Groups.Rules;
using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Groups;

/// <summary>
/// Represents a group in the system.
/// </summary>
public class Group : Entity
{
    public string Name { get; private set; }
    public string LeaderId { get; private set; }
    public byte[]? RowVersion { get; private set; }

    private readonly List<User> _users = new();
    public virtual IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private readonly List<GroupInvite> _invites = new();
    public virtual IReadOnlyCollection<GroupInvite> Invites => _invites.AsReadOnly();

    protected Group() { }

    public Group(string name, User leader)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Group name is required");
        if (leader == null)
            throw new ArgumentNullException(nameof(leader));

        Name = name;
        LeaderId = leader.Id;

        // Regra de Negócio: O Líder já entra como membro automaticamente?
        _users.Add(leader);
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Group name is required");
        }
        Name = newName;
    }

    public void AddMember(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        CheckRule(new GroupCannotHaveMoreThanMaxMembersRule(_users.Count));

        if (_users.Any(u => u.Id == user.Id))
        {
            return;
        }

        _users.Add(user);
    }
}
