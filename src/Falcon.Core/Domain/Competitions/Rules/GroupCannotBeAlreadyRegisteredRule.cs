using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: Group cannot be already registered in the competition.
/// </summary>
public class GroupCannotBeAlreadyRegisteredRule : IBusinessRule
{
    private readonly bool _isAlreadyRegistered;

    public GroupCannotBeAlreadyRegisteredRule(bool isAlreadyRegistered)
    {
        _isAlreadyRegistered = isAlreadyRegistered;
    }

    public bool IsBroken()
    {
        return _isAlreadyRegistered;
    }

    public string Message => "O grupo já está registrado nesta competição.";
}
