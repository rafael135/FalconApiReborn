namespace Falcon.Api.Features.Auth.Shared;

/// <summary>
/// Basic user DTO returned after authentication/registration.
/// </summary>
/// <param name="Id">User identifier.</param>
/// <param name="Name">Full name.</param>
/// <param name="Email">User's email address.</param>
/// <param name="Ra">Academic registration (RA).</param>
/// <param name="Role">User role (Student, Teacher, Admin).</param>
/// <param name="Group">User's group information, if any.</param>"}]} }]}]}
public record UserDto(
    string Id,
    string Name,
    string Email,
    string Ra,
    string Role,
    GroupDto? Group
);
