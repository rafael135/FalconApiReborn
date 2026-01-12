namespace Falcon.Api.Features.Competitions.UnregisterGroup;

/// <summary>
/// Result of unregistering a group from a competition.
/// </summary>
/// <param name="Success">True if the operation succeeded.</param>
/// <param name="Message">Human-friendly message describing the outcome.</param>
public record UnregisterGroupResult(bool Success, string Message); 
