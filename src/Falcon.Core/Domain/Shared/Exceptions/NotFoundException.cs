namespace Falcon.Core.Domain.Shared.Exceptions;

/// <summary>
/// Exception thrown when a requested entity could not be found.
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>
    /// Creates a new <see cref="NotFoundException"/> for the specified entity name and key.
    /// </summary>
    /// <param name="name">The entity name.</param>
    /// <param name="key">The key that was not found.</param>
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.") { }
}
