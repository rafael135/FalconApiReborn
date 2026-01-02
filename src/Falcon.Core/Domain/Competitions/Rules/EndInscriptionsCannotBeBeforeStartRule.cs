using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: End of inscriptions cannot be before start of inscriptions.
/// </summary>
public class EndInscriptionsCannotBeBeforeStartRule : IBusinessRule
{
    private readonly DateTime _startInscriptions;
    private readonly DateTime _endInscriptions;

    public EndInscriptionsCannotBeBeforeStartRule(DateTime startInscriptions, DateTime endInscriptions)
    {
        _startInscriptions = startInscriptions;
        _endInscriptions = endInscriptions;
    }

    public bool IsBroken()
    {
        return _endInscriptions < _startInscriptions;
    }

    public string Message => "O fim das inscrições não pode ser anterior ao início das inscrições.";
}
