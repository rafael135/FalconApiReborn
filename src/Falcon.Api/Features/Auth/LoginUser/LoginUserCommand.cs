using MediatR;

namespace Falcon.Api.Features.Auth.LoginUser;

public record LoginUserCommand(string RA, string Password) : IRequest<LoginUserResult>;
