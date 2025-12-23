namespace Falcon.Core.Domain.Shared;

public interface IBusinessRule
{
    /// <summary>
    /// Determines whether the business rule is broken.
    /// </summary>
    /// <returns>True if the business rule is broken; otherwise, false.</returns>
    bool IsBroken();

    /// <summary>
    /// Gets the message associated with the business rule.
    /// </summary>
    string Message { get; }
}
