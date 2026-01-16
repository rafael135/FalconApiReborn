using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Groups.Rules;
using Falcon.Core.Domain.Users;

namespace Falcon.Core.Domain.Groups;

/// <summary>
/// Represents a group of users used for participating in competitions and sharing resources.
/// </summary>
public class Group : Entity
{
    /// <summary>
    /// The group's display name.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The identifier of the group's leader (user id).
    /// </summary>
    public string LeaderId { get; private set; }

    /// <summary>
    /// Row version used for optimistic concurrency.
    /// </summary>
    public byte[]? RowVersion { get; private set; }

    private readonly List<User> _users = new();

    /// <summary>
    /// Read-only collection of users who are members of the group.
    /// </summary>
    public virtual IReadOnlyCollection<User> Users => _users.AsReadOnly();

    private readonly List<GroupInvite> _invites = new();

    /// <summary>
    /// Pending invites for the group.
    /// </summary>
    public virtual IReadOnlyCollection<GroupInvite> Invites => _invites.AsReadOnly();

    private readonly List<GroupInCompetition> _groupsInCompetitions = new();

    /// <summary>
    /// Associations between this group and competitions.
    /// </summary>
    public virtual IReadOnlyCollection<GroupInCompetition> GroupsInCompetitions =>
        _groupsInCompetitions.AsReadOnly();

    private readonly List<Competition> _competitions = new();

    /// <summary>
    /// Competitions where this group participates.
    /// </summary>
    public virtual IReadOnlyCollection<Competition> Competitions => _competitions.AsReadOnly();

    private readonly List<Competitions.CompetitionRanking> _rankings = new();

    /// <summary>
    /// Ranking entries for this group across competitions.
    /// </summary>
    public virtual IReadOnlyCollection<Competitions.CompetitionRanking> Rankings =>
        _rankings.AsReadOnly();

    private readonly List<Exercises.GroupExerciseAttempt> _attempts = new();

    /// <summary>
    /// Attempts performed by this group.
    /// </summary>
    public virtual IReadOnlyCollection<Exercises.GroupExerciseAttempt> Attempts =>
        _attempts.AsReadOnly();

    private readonly List<Auditing.Log> _logs = new();

    /// <summary>
    /// Audit logs related to this group.
    /// </summary>
    public virtual IReadOnlyCollection<Auditing.Log> Logs => _logs.AsReadOnly();

#pragma warning disable CS8618
    protected Group() { }
#pragma warning restore CS8618

    /// <summary>
    /// Creates a new group with the provided <paramref name="name"/> and leader.
    /// The leader is automatically added to the group's members.
    /// </summary>
    /// <param name="name">The group name.</param>
    /// <param name="leader">The initial leader and member.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="leader"/> is null.</exception>
    public Group(string name, User leader)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do grupo é obrigatório");
        if (leader == null)
            throw new ArgumentNullException(nameof(leader));

        Name = name;
        LeaderId = leader.Id;

        // The leader is added as a member by default
        _users.Add(leader);
    }

    /// <summary>
    /// Renames the group.
    /// </summary>
    /// <param name="newName">The new name for the group.</param>
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Nome do grupo é obrigatório");
        }
        Name = newName;
    }

    /// <summary>
    /// Adds a member to the group. Enforces the business rule limiting group size.
    /// </summary>
    /// <param name="user">The user to add.</param>
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

    /// <summary>
    /// Removes a member from the group.
    /// </summary>
    /// <param name="user">The member to remove.</param>
    public void RemoveMember(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _users.Remove(user);
    }

    /// <summary>
    /// Transfers group leadership to another existing member.
    /// </summary>
    /// <param name="newLeader">The new leader of the group (must be a member).</param>
    public void TransferLeadership(User newLeader)
    {
        if (newLeader == null)
        {
            throw new ArgumentNullException(nameof(newLeader));
        }

        if (!_users.Any(u => u.Id == newLeader.Id))
        {
            throw new InvalidOperationException("O novo líder deve ser um membro do grupo");
        }

        LeaderId = newLeader.Id;
    }

    /// <summary>
    /// Disbands the group and clears group associations from its members.
    /// </summary>
    public void Disband()
    {
        var usersToRemove = _users.ToList();
        foreach (var user in usersToRemove)
        {
            user.LeaveGroup();
        }
        _users.Clear();
    }
}
