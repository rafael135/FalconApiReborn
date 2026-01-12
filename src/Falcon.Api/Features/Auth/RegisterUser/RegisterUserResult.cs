namespace Falcon.Api.Features.Auth.RegisterUser;

/// <summary>
/// Result returned after user registration.
/// </summary>
/// <param name="Id">User identifier.</param>
/// <param name="Name">User's full name.</param>
/// <param name="Email">User's email address.</param>
/// <param name="Ra">Academic registration (RA).</param>
/// <param name="Role">User role.</param>
/// <param name="Token">JWT authentication token.</param>
/// <param name="EmailConfirmed">Indicates whether the email is confirmed.</param>
/// <param name="joinYear">Year of joining (optional).</param>
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
