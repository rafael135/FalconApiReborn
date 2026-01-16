using System.Security.Claims;
using Falcon.Api.Features.Users.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Users.GetUser;

/// <summary>
/// Handler for retrieving detailed user information.
/// Supports self-access or admin access authorization.
/// </summary>
public class GetUserHandler : IRequestHandler<GetUserQuery, GetUserResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetUserHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserHandler"/> class.
    /// </summary>
    /// <param name="userManager">The user manager for accessing user data.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor for authorization checks.</param>
    /// <param name="logger">The logger instance.</param>
    public GetUserHandler(
        UserManager<Core.Domain.Users.User> userManager,
        IHttpContextAccessor httpContextAccessor,
        ILogger<GetUserHandler> logger
    )
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Handles the get user query.
    /// </summary>
    /// <param name="request">The query containing the user ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The detailed user information.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authorized to access the information.</exception>
    /// <exception cref="NotFoundException">Thrown when the user is not found.</exception>
    public async Task<GetUserResult> Handle(
        GetUserQuery request,
        CancellationToken cancellationToken
    )
    {
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

        // Fetch user with group navigation
        var user = await _userManager
            .Users.Include(u => u.Group)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException(nameof(Core.Domain.Users.User), request.UserId);
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        _logger.LogInformation(
            "User {UserId} retrieved profile for user {TargetUserId}",
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
            user.Group?.Name,
            roles.ToList(),
            user.CreatedAt,
            user.LastLoggedAt
        );

        return new GetUserResult(userDto);
    }
}
