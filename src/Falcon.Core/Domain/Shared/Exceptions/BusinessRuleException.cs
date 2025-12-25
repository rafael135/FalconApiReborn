namespace Falcon.Core.Domain.Shared.Exceptions;

public class BusinessRuleException : DomainException
{
    public IBusinessRule BrokenRule { get; }

    public BusinessRuleException(IBusinessRule brokenRule)
        : base(brokenRule.Message)
    {
        BrokenRule = brokenRule;
    }

    public override string ToString()
    {
        return $"{BrokenRule.GetType().Name}: {BrokenRule.Message}";
    }
}
