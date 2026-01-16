using Falcon.Core.Domain.Users;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Admin.GetUsers;

/// <summary>
/// User summary information for admin listing.
/// </summary>
public record UserSummaryDto(
    string Id,
    string Name,
    string Email,
    string RA,
    int? JoinYear,
    string? Department,
    DateTime CreatedAt,
    DateTime LastLoggedAt,
    Guid? GroupId,
    string? GroupName,
    List<string> Roles
);

/// <summary>
/// Handler for getting all users (with optional role filter).
/// </summary>
public class GetUsersHandler : IRequestHandler<GetUsersQuery, GetUsersResult>
{
    private readonly FalconDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<GetUsersHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUsersHandler"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <param name="userManager">The user manager for role retrieval.</param>
    /// <param name="logger">The logger instance.</param>
    public GetUsersHandler(
        FalconDbContext context,
        UserManager<User> userManager,
        ILogger<GetUsersHandler> logger
    )
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Handles the users listing query with optional role and search filters.
    /// </summary>
    /// <param name="request">The query containing filters and pagination.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The paginated users list.</returns>
    public async Task<GetUsersResult> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _context.Users.Include(u => u.Group).AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            var roleName = request.Role.Trim();
            query = query.Where(u =>
                _context.UserRoles.Any(ur =>
                    ur.UserId == u.Id
                    && _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == roleName)
                )
            );
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchLower = request.Search.Trim().ToLowerInvariant();
            query = query.Where(u =>
                u.Name.ToLowerInvariant().Contains(searchLower)
                || u.Email!.ToLowerInvariant().Contains(searchLower)
                || u.RA.ToLowerInvariant().Contains(searchLower)
            );
        }

        var total = await query.CountAsync(cancellationToken);

        var paginatedUsers = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        // Get roles for each user
        var userSummaries = new List<UserSummaryDto>();
        foreach (var user in paginatedUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);

            userSummaries.Add(
                new UserSummaryDto(
                    user.Id,
                    user.Name,
                    user.Email!,
                    user.RA,
                    user.JoinYear,
                    user.Department,
                    user.CreatedAt,
                    user.LastLoggedAt,
                    user.GroupId,
                    user.Group?.Name,
                    roles.ToList()
                )
            );
        }

        _logger.LogInformation(
            "Retrieved {Count} users (total: {Total})",
            userSummaries.Count,
            total
        );

        return new GetUsersResult(userSummaries, total, request.Skip, request.Take);
    }
}
