using Falcon.Core.Domain.Shared;
using Falcon.Core.Domain.Shared.Exceptions;

namespace Falcon.Core;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity() { }

    protected Entity(Guid id)
    {
        Id = id;
    }

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

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    protected static void CheckRule(IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BusinessRuleException(rule);
        }
    }
}
