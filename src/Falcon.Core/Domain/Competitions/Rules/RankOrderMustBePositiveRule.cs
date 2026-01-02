using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: Rank order must be greater than zero.
/// </summary>
public class RankOrderMustBePositiveRule : IBusinessRule
{
    private readonly int _rankOrder;

    public RankOrderMustBePositiveRule(int rankOrder)
    {
        _rankOrder = rankOrder;
    }

    public bool IsBroken()
    {
        return _rankOrder <= 0;
    }

    public string Message => "A posição no ranking deve ser maior que 0.";
}
