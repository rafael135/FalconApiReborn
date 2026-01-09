using Falcon.Api.Features.Users.Shared;

namespace Falcon.Api.Features.Users.GetUser;

/// <summary>
/// Result containing detailed user information.
/// </summary>
/// <param name="User">The detailed user information.</param>
public record GetUserResult(UserDetailDto User);
