using Falcon.Api.Features.Users.Shared;

namespace Falcon.Api.Features.Users.UpdateUser;

/// <summary>
/// Result containing the updated user information.
/// </summary>
/// <param name="User">The updated user information.</param>
public record UpdateUserResult(UserDetailDto User);
