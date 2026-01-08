using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Groups.Rules;

public class GroupCannotHaveMoreThanMaxMembersRule : IBusinessRule
{
    private readonly int _currentMemberCount;
    private const int MaxMembers = 3;

    public GroupCannotHaveMoreThanMaxMembersRule(int currentMemberCount)
    {
        _currentMemberCount = currentMemberCount;
    }

    public bool IsBroken()
    {
        return _currentMemberCount >= MaxMembers;
    }

    public string Message => $"O grupo não pode ter mais de {MaxMembers} membros.";
}
