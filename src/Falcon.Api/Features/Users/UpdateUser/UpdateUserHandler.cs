using System.Security.Claims;
using Falcon.Api.Features.Users.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Users.UpdateUser;

/// <summary>
/// Handler for updating user profile information.
/// Validates email and RA uniqueness, handles password changes, and enforces self-or-admin authorization.
/// </summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UpdateUserHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateUserHandler"/> class.
    /// </summary>
    /// <param name="userManager">The user manager for accessing and updating user data.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor for authorization checks.</param>
    /// <param name="logger">The logger instance.</param>
    public UpdateUserHandler(
        UserManager<Core.Domain.Users.User> userManager,
        IHttpContextAccessor httpContextAccessor,
        ILogger<UpdateUserHandler> logger
    )
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Handles the update user command.
    /// </summary>
    /// <param name="request">The command containing the updated user information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated user information.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authorized to update the profile.</exception>
    /// <exception cref="NotFoundException">Thrown when the user is not found.</exception>
    /// <exception cref="FormException">Thrown when validation fails.</exception>
    public async Task<UpdateUserResult> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken
    )
    {
        var errors = new Dictionary<string, string>();

        // Basic validation
        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add("name", "Nome é obrigatório");
        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add("email", "E-mail é obrigatório");
        if (string.IsNullOrWhiteSpace(request.RA))
            errors.Add("ra", "RA é obrigatório");

        if (errors.Any())
            throw new FormException(errors);

        // Get current user ID from claims
        var httpContext =
            _httpContextAccessor.HttpContext
            ?? throw new UnauthorizedAccessException("Usuário não autenticado");

        var currentUserId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var isAdmin = httpContext.User.IsInRole("Admin");

        // Authorization: self or admin only
        if (currentUserId != request.UserId && !isAdmin)
        {
            throw new UnauthorizedAccessException("Acesso negado");
        }

        // Fetch user
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException(nameof(Core.Domain.Users.User), request.UserId);
        }

        // Validate email uniqueness (excluding current user)
        var existingEmail = await _userManager.Users.AnyAsync(
            u => u.Email == request.Email && u.Id != request.UserId,
            cancellationToken
        );
        if (existingEmail)
        {
            errors.Add("email", "E-mail já utilizado");
        }

        // Validate RA uniqueness (excluding current user)
        var existingRA = await _userManager.Users.AnyAsync(
            u => u.RA == request.RA && u.Id != request.UserId,
            cancellationToken
        );
        if (existingRA)
        {
            errors.Add("ra", "RA já cadastrado");
        }

        // Validate password change requirements
        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            if (string.IsNullOrEmpty(request.CurrentPassword))
            {
                errors.Add("currentPassword", "Senha atual é obrigatória para alterar a senha");
            }
            else
            {
                var passwordValid = await _userManager.CheckPasswordAsync(
                    user,
                    request.CurrentPassword
                );
                if (!passwordValid)
                {
                    errors.Add("currentPassword", "Senha atual incorreta");
                }
            }
        }

        if (errors.Any())
            throw new FormException(errors);

        // Update profile using domain method
        user.UpdateProfile(request.Name, request.RA, request.JoinYear, request.Department);

        // Update email if changed (requires UserManager for Identity fields)
        if (user.Email != request.Email)
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, request.Email);
            if (!setEmailResult.Succeeded)
                throw new FormException(setEmailResult.Errors.ToFriendlyDictionary());

            var setUserNameResult = await _userManager.SetUserNameAsync(user, request.Email);
            if (!setUserNameResult.Succeeded)
                throw new FormException(setUserNameResult.Errors.ToFriendlyDictionary());
        }

        // Update password if provided
        if (
            !string.IsNullOrEmpty(request.NewPassword)
            && !string.IsNullOrEmpty(request.CurrentPassword)
        )
        {
            var changePasswordResult = await _userManager.ChangePasswordAsync(
                user,
                request.CurrentPassword,
                request.NewPassword
            );
            if (!changePasswordResult.Succeeded)
                throw new FormException(changePasswordResult.Errors.ToFriendlyDictionary());
        }

        // Save changes
        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new FormException(updateResult.Errors.ToFriendlyDictionary());

        // Get updated roles
        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation(
            "User {UserId} updated profile for user {TargetUserId}",
            currentUserId,
            request.UserId
        );

        var userDto = new UserDetailDto(
            user.Id,
            user.Name,
            user.Email!,
            user.RA,
            user.JoinYear,
            user.Department,
            user.GroupId,
            null, // Group name not needed in update result
            roles.ToList(),
            user.CreatedAt,
            user.LastLoggedAt
        );

        return new UpdateUserResult(userDto);
    }
}
