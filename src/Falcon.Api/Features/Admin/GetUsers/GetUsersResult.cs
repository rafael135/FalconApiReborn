namespace Falcon.Api.Features.Admin.GetUsers;

/// <summary>
/// Result containing paginated list of users.
/// </summary>
public record GetUsersResult(List<UserSummaryDto> Users, int Total, int Skip, int Take);
