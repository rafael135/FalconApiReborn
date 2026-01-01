namespace Falcon.Api.Features.Competitions.UnregisterGroup;

/// <summary>
/// Result of unregistering a group from a competition.
/// </summary>
public record UnregisterGroupResult(bool Success, string Message);
