namespace Falcon.Api.Features.Groups.Shared;

/// <summary>
/// Summary information about a user.
/// </summary>
public record UserSummaryDto(
    string Id,
    string Name,
    string Email,
    string Ra,
    int? JoinYear,
    string? Department
);
