using Falcon.Api.Features.Users.Shared;
using MediatR;

namespace Falcon.Api.Features.Users.UpdateUser;

/// <summary>
/// Command for updating user profile information.
/// </summary>
/// <param name="UserId">The unique identifier of the user to update.</param>
/// <param name="Name">The updated name of the user.</param>
/// <param name="Email">The updated email address of the user.</param>
/// <param name="RA">The updated RA (student registration number) of the user.</param>
/// <param name="JoinYear">An optional property of type int representing the updated year the user joined.</param>
/// <param name="Department">An optional property of type string representing the updated user's department.</param>
/// <param name="CurrentPassword">An optional property of type string representing the current password (required if changing password).</param>
/// <param name="NewPassword">An optional property of type string representing the new password.</param>
public record UpdateUserCommand(
    string UserId,
    string Name,
    string Email,
    string RA,
    int? JoinYear,
    string? Department,
    string? CurrentPassword,
    string? NewPassword
) : IRequest<UpdateUserResult>;
