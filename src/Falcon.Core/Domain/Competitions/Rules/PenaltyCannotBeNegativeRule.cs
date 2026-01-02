using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: Penalty cannot be negative.
/// </summary>
public class PenaltyCannotBeNegativeRule : IBusinessRule
{
    private readonly double _penalty;

    public PenaltyCannotBeNegativeRule(double penalty)
    {
        _penalty = penalty;
    }

    public bool IsBroken()
    {
        return _penalty < 0;
    }

    public string Message => "A penalidade não pode ser negativa.";
}
