namespace Falcon.Api.Features.Competitions.AddExercise;

/// <summary>
/// Result of an attempt to add an exercise to a competition.
/// </summary>
/// <param name="Success">True if the operation succeeded.</param>
/// <param name="Message">Human-friendly message describing the result.</param>
public record AddExerciseResult(bool Success, string Message);
