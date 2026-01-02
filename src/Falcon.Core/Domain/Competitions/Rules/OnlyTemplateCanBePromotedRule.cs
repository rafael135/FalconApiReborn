using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: Only template competitions can be promoted to active competitions.
/// </summary>
public class OnlyTemplateCanBePromotedRule : IBusinessRule
{
    private readonly CompetitionStatus _currentStatus;

    public OnlyTemplateCanBePromotedRule(CompetitionStatus currentStatus)
    {
        _currentStatus = currentStatus;
    }

    public bool IsBroken()
    {
        return _currentStatus != CompetitionStatus.ModelTemplate;
    }

    public string Message => "Apenas competições modelo podem ser promovidas a competições ativas.";
}
