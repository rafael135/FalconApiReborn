using Falcon.Api.Features.Auth.Shared;

namespace Falcon.Api.Features.Auth.LoginUser;

public record LoginUserResult(UserDto User, string Token);
