using Falcon.Core.Domain.Competitions;
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

    private readonly List<GroupInCompetition> _groupsInCompetitions = new();
    public virtual IReadOnlyCollection<GroupInCompetition> GroupsInCompetitions => _groupsInCompetitions.AsReadOnly();

    private readonly List<Competition> _competitions = new();
    public virtual IReadOnlyCollection<Competition> Competitions => _competitions.AsReadOnly();

    private readonly List<Competitions.CompetitionRanking> _rankings = new();
    public virtual IReadOnlyCollection<Competitions.CompetitionRanking> Rankings => _rankings.AsReadOnly();

    private readonly List<Exercises.GroupExerciseAttempt> _attempts = new();
    public virtual IReadOnlyCollection<Exercises.GroupExerciseAttempt> Attempts => _attempts.AsReadOnly();

    private readonly List<Auditing.Log> _logs = new();
    public virtual IReadOnlyCollection<Auditing.Log> Logs => _logs.AsReadOnly();

#pragma warning disable CS8618
    protected Group() { }
#pragma warning restore CS8618

    public Group(string name, User leader)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Nome do grupo é obrigatório");
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
            throw new ArgumentException("Nome do grupo é obrigatório");
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

    public void RemoveMember(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        _users.Remove(user);
    }

    /// <summary>
    /// Transfers group leadership to another member.
    /// </summary>
    /// <param name="newLeader">The new leader of the group. Must be an existing member.</param>
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
    /// Disbands the group by removing all members and clearing their group association.
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
