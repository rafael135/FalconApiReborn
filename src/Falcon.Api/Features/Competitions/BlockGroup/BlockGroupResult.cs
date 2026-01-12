namespace Falcon.Api.Features.Competitions.BlockGroup;

/// <summary>
/// Result of blocking a group in a competition.
/// </summary>
/// <param name="Success">True if block operation succeeded.</param>
/// <param name="Message">Human-friendly message.</param>
public record BlockGroupResult(bool Success, string Message); 
