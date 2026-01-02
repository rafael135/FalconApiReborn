using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: Points cannot be negative.
/// </summary>
public class PointsCannotBeNegativeRule : IBusinessRule
{
    private readonly double _points;

    public PointsCannotBeNegativeRule(double points)
    {
        _points = points;
    }

    public bool IsBroken()
    {
        return _points < 0;
    }

    public string Message => "Os pontos não podem ser negativos.";
}
