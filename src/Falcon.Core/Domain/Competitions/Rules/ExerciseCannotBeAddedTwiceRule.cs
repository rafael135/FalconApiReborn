using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Competitions.Rules;

/// <summary>
/// Business rule: Exercise cannot be added twice to the same competition.
/// </summary>
public class ExerciseCannotBeAddedTwiceRule : IBusinessRule
{
    private readonly bool _isAlreadyAdded;

    public ExerciseCannotBeAddedTwiceRule(bool isAlreadyAdded)
    {
        _isAlreadyAdded = isAlreadyAdded;
    }

    public bool IsBroken()
    {
        return _isAlreadyAdded;
    }

    public string Message => "O exercício já foi adicionado a esta competição.";
}
