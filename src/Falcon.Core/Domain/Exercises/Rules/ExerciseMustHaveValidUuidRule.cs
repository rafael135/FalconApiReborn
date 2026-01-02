using Falcon.Core.Domain.Shared;

namespace Falcon.Core.Domain.Exercises.Rules;

/// <summary>
/// Business rule: Exercise must have a valid Judge UUID.
/// </summary>
public class ExerciseMustHaveValidUuidRule : IBusinessRule
{
    private readonly string _judgeUuid;

    public ExerciseMustHaveValidUuidRule(string judgeUuid)
    {
        _judgeUuid = judgeUuid;
    }

    public bool IsBroken()
    {
        return string.IsNullOrWhiteSpace(_judgeUuid);
    }

    public string Message => "O UUID do Judge é obrigatório.";
}
