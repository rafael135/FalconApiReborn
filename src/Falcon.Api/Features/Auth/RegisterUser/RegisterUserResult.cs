namespace Falcon.Api.Features.Auth.RegisterUser;

public record RegisterUserResult(
    string Id,
    string Name,
    string Email,
    string Ra,
    string Role,
    string Token,
    bool EmailConfirmed,
    int? joinYear
);
