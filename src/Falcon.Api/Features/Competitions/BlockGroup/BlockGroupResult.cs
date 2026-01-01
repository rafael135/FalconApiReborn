namespace Falcon.Api.Features.Competitions.BlockGroup;

/// <summary>
/// Result of blocking a group in a competition.
/// </summary>
public record BlockGroupResult(bool Success, string Message);
