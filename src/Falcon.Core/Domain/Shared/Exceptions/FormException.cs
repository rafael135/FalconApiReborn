namespace Falcon.Core.Domain.Shared.Exceptions;

/// <summary>
/// Exception that represents a collection of validation errors for form-like inputs.
/// </summary>
public class FormException : DomainException
{
    /// <summary>A dictionary mapping field keys to error messages.</summary>
    public Dictionary<string, string> Errors { get; }

    /// <summary>Creates a new <see cref="FormException"/> with the provided errors.</summary>
    /// <param name="errors">A dictionary of field errors.</param>
    public FormException(Dictionary<string, string> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
