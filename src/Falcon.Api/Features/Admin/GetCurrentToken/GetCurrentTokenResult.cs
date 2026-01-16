namespace Falcon.Api.Features.Admin.GetCurrentToken;

/// <summary>
/// Result containing current teacher token or null if not found/expired.
/// </summary>
public record GetCurrentTokenResult(string? Token);
