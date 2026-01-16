namespace Falcon.Core.Domain.Shared.Exceptions;

/// <summary>
/// Exception thrown when a domain business rule is violated.
/// </summary>
public class BusinessRuleException : DomainException
{
    /// <summary>The broken business rule.</summary>
    public IBusinessRule BrokenRule { get; }

    /// <summary>Creates a new <see cref="BusinessRuleException"/> for the specified broken rule.</summary>
    /// <param name="brokenRule">The rule that was violated.</param>
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
