namespace Falcon.Api.Features.Exercises.Shared;

/// <summary>
/// Test case information (input and expected output).
/// </summary>
public record TestCaseDto(
    Guid InputId,
    Guid OutputId,
    string InputContent,
    string ExpectedOutput
);
