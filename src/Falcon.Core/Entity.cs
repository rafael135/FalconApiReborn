using Falcon.Core.Domain.Shared;
using Falcon.Core.Domain.Shared.Exceptions;

namespace Falcon.Core;

/// <summary>
/// Base class for all entities in the domain.
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// The unique identifier of the entity.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class.
    /// </summary>
    protected Entity() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity"/> class with the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier for the entity.</param>
    protected Entity(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns><c>true</c> if the specified object is equal to the current entity; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (Id == Guid.Empty || other.Id == Guid.Empty)
        {
            return false;
        }

        return Id == other.Id;
    }

    /// <summary>
    /// Returns a hash code for the entity.
    /// </summary>
    /// <returns>A hash code for the current entity.</returns>
    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    /// <summary>
    /// Checks the specified business rule and throws a <see cref="BusinessRuleException"/> if the rule is broken.
    /// </summary>
    /// <param name="rule">The business rule to check.</param>
    /// <exception cref="BusinessRuleException">Thrown when the business rule is broken.</exception>
    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleException(rule);
        }
    }
}
