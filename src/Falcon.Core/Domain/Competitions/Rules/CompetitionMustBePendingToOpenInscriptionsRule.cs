using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: Competition must be in Pending status to open inscriptions.
/// </summary>
public class CompetitionMustBePendingToOpenInscriptionsRule : IBusinessRule
{
    private readonly CompetitionStatus _currentStatus;

    public CompetitionMustBePendingToOpenInscriptionsRule(CompetitionStatus currentStatus)
    {
        _currentStatus = currentStatus;
    }

    public bool IsBroken()
    {
        return _currentStatus != CompetitionStatus.Pending;
    }

    public string Message => "A competição deve estar no estado Pendente para abrir inscrições.";
}
