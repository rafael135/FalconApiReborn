namespace Falcon.Api.Features.Competitions.RemoveExercise;

/// <summary>
/// Result of a remove exercise operation.
/// </summary>
/// <param name="Success">True when the operation succeeded.</param>
/// <param name="Message">Human-friendly message describing the outcome.</param>
public record RemoveExerciseResult(bool Success, string Message);
