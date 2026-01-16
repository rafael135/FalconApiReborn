namespace Falcon.Core.Domain.Shared.Exceptions;

/// <summary>
/// Base exception class for domain-level errors.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// Creates a new <see cref="DomainException"/> with a message.
    /// </summary>
    /// <param name="message">Error message.</param>
    public DomainException(string message)
        : base(message) { }
}
