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

    public GetUsersHandler(
        FalconDbContext context,
        UserManager<User> userManager,
        ILogger<GetUsersHandler> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<GetUsersResult> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<User> users;

        // Filter by role if specified
        if (!string.IsNullOrEmpty(request.Role))
        {
            users = await _userManager.GetUsersInRoleAsync(request.Role);
        }
        else
        {
            users = await _context.Users
                .Include(u => u.Group)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        // Apply search filter if specified
        if (!string.IsNullOrEmpty(request.Search))
        {
            var searchLower = request.Search.ToLower();
            users = users.Where(u =>
                u.Name.ToLower().Contains(searchLower) ||
                u.Email!.ToLower().Contains(searchLower) ||
                u.RA.ToLower().Contains(searchLower));
        }

        // Order by creation date descending
        var orderedUsers = users.OrderByDescending(u => u.CreatedAt);

        // Get total count
        var total = orderedUsers.Count();

        // Apply pagination
        var paginatedUsers = orderedUsers
            .Skip(request.Skip)
            .Take(request.Take)
            .ToList();

        // Get roles for each user
        var userSummaries = new List<UserSummaryDto>();
        foreach (var user in paginatedUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);

            userSummaries.Add(new UserSummaryDto(
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
            ));
        }

        _logger.LogInformation("Retrieved {Count} users (total: {Total})", userSummaries.Count, total);

        return new GetUsersResult(userSummaries, total, request.Skip, request.Take);
    }
}
