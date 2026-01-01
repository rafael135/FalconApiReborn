using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: Inscriptions must be open for groups to register.
/// </summary>
public class InscriptionsMustBeOpenRule : IBusinessRule
{
    private readonly bool _isInscriptionOpen;

    public InscriptionsMustBeOpenRule(bool isInscriptionOpen)
    {
        _isInscriptionOpen = isInscriptionOpen;
    }

    public bool IsBroken()
    {
        return !_isInscriptionOpen;
    }

    public string Message => "As inscrições não estão abertas para esta competição.";
}
