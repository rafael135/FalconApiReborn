using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Groups.Rules;

/// <summary>
/// Business rule that ensures a group cannot have more than the configured maximum number of members.
/// </summary>
public class GroupCannotHaveMoreThanMaxMembersRule : IBusinessRule
{
    private readonly int _currentMemberCount;
    private const int MaxMembers = 3;

    /// <summary>Creates the rule with the current member count.</summary>
    public GroupCannotHaveMoreThanMaxMembersRule(int currentMemberCount)
    {
        _currentMemberCount = currentMemberCount;
    }

    /// <inheritdoc />
    public bool IsBroken()
    {
        return _currentMemberCount >= MaxMembers;
    }

    /// <inheritdoc />
    public string Message => $"O grupo não pode ter mais de {MaxMembers} membros.";
}
