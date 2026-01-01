using MediatR;

namespace Falcon.Api.Features.Auth.RegisterUser;

public record RegisterUserCommand(
    string Ra,
    string Name,
    string Email,
    string Role,
    string Password,
    string? Department = null,
    string? AccessCode = null,
    int? joinYear = null
) : IRequest<RegisterUserResult>;
