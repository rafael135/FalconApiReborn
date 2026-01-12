using Falcon.Api.Features.Auth.Shared;

namespace Falcon.Api.Features.Auth.LoginUser;

/// <summary>
/// Result returned after authentication.
/// </summary>
/// <param name="User">Basic user information.</param>
/// <param name="Token">JWT authentication token.</param>
public record LoginUserResult(UserDto User, string Token);
