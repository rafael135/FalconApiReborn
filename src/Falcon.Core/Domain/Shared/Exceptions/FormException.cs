namespace Falcon.Core.Domain.Shared.Exceptions;

public class FormException : DomainException
{
    public Dictionary<string, string> Errors { get; }

    public FormException(Dictionary<string, string> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
