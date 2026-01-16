using System.Security.Claims;
using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Users.DeleteUser;

/// <summary>
/// Handler for soft deleting a user.
/// Handles group membership, leadership transfer, and audit logging.
/// </summary>
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResult>
{
    private readonly UserManager<Core.Domain.Users.User> _userManager;
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DeleteUserHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteUserHandler"/> class.
    /// </summary>
    /// <param name="userManager">The user manager for accessing user data.</param>
    /// <param name="dbContext">The database context.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor for authorization checks.</param>
    /// <param name="logger">The logger instance.</param>
    public DeleteUserHandler(
        UserManager<Core.Domain.Users.User> userManager,
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DeleteUserHandler> logger
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Handles the delete user command.
    /// </summary>
    /// <param name="request">The command containing the user ID to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result indicating successful deletion.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user is not authorized to delete the account.</exception>
    /// <exception cref="NotFoundException">Thrown when the user is not found.</exception>
    /// <exception cref="BusinessRuleException">Thrown when business rules prevent deletion.</exception>
    public async Task<DeleteUserResult> Handle(
        DeleteUserCommand request,
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

        // Fetch user from DbContext to ensure EF Core tracks changes
        var user = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            cancellationToken
        );

        if (user == null)
        {
            throw new NotFoundException(nameof(Core.Domain.Users.User), request.UserId);
        }

        // Handle group membership
        if (user.GroupId != null)
        {
            var group = await _dbContext
                .Groups.Include(g => g.Users)
                .FirstOrDefaultAsync(g => g.Id == user.GroupId, cancellationToken);

            if (group != null)
            {
                // Check if user is the group leader
                if (group.LeaderId == user.Id)
                {
                    var otherMembers = group.Users.Where(u => u.Id != user.Id).ToList();

                    if (otherMembers.Any())
                    {
                        // Transfer leadership to first member alphabetically by name
                        var newLeader = otherMembers.OrderBy(u => u.Name).First();
                        group.TransferLeadership(newLeader);

                        _logger.LogInformation(
                            "Leadership of group {GroupId} transferred from {OldLeaderId} to {NewLeaderId}",
                            group.Id,
                            user.Id,
                            newLeader.Id
                        );
                    }
                    else
                    {
                        // User is the only member, disband the group
                        group.Disband();

                        _logger.LogInformation(
                            "Group {GroupId} disbanded due to sole leader {UserId} deletion",
                            group.Id,
                            user.Id
                        );
                    }
                }

                // Remove user from group (works for both leader and non-leader)
                group.RemoveMember(user);
                user.LeaveGroup();
            }
        }

        // Create audit log entry before soft deletion
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var log = new Log(
            LogType.DeleteUser,
            ipAddress,
            user,
            null, // Group is already cleared from user
            null
        );
        await _dbContext.Logs.AddAsync(log, cancellationToken);

        // Soft delete the user
        user.SoftDelete();

        // Save all changes
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "User {UserId} soft deleted by {CurrentUserId}",
            request.UserId,
            currentUserId
        );

        return new DeleteUserResult("Usuário excluído com sucesso");
    }
}
