using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: Group must have exactly the required number of members for the competition.
/// </summary>
public class GroupMustHaveRequiredMembersRule : IBusinessRule
{
    private readonly int _groupMemberCount;
    private readonly int _requiredMembers;

    public GroupMustHaveRequiredMembersRule(int groupMemberCount, int requiredMembers)
    {
        _groupMemberCount = groupMemberCount;
        _requiredMembers = requiredMembers;
    }

    public bool IsBroken()
    {
        return _groupMemberCount != _requiredMembers;
    }

    public string Message => $"O grupo deve ter exatamente {_requiredMembers} membros para participar desta competição. Membros atuais: {_groupMemberCount}.";
}
