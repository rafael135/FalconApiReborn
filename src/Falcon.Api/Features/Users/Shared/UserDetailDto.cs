namespace Falcon.Api.Features.Users.Shared;

/// <summary>
/// Represents detailed user information including group membership and roles.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Name">The name of the user.</param>
/// <param name="Email">The email address of the user.</param>
/// <param name="RA">The RA (student registration number) of the user.</param>
/// <param name="JoinYear">An optional property of type int representing the year the user joined.</param>
/// <param name="Department">An optional property of type string representing the user's department.</param>
/// <param name="GroupId">An optional property of type Guid representing the ID of the user's group.</param>
/// <param name="GroupName">An optional property of type string representing the name of the user's group.</param>
/// <param name="Roles">A list of roles assigned to the user.</param>
/// <param name="CreatedAt">The date and time when the user was created.</param>
/// <param name="LastLoggedAt">The date and time when the user last logged in.</param>
public record UserDetailDto(
    string Id,
    string Name,
    string Email,
    string RA,
    int? JoinYear,
    string? Department,
    Guid? GroupId,
    string? GroupName,
    List<string> Roles,
    DateTime CreatedAt,
    DateTime LastLoggedAt
);
