using Falcon.Api.Features.Logs.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Users;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Logs.GetUserLogs;

/// <summary>
/// Handler for getting a specific user's logs.
/// </summary>
public class GetUserLogsHandler : IRequestHandler<GetUserLogsQuery, GetUserLogsResult>
{
    private readonly FalconDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<GetUserLogsHandler> _logger;

    public GetUserLogsHandler(
        FalconDbContext context,
        UserManager<User> userManager,
        ILogger<GetUserLogsHandler> logger
    )
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<GetUserLogsResult> Handle(
        GetUserLogsQuery request,
        CancellationToken cancellationToken
    )
    {
        // Verify user exists
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        var query = _context
            .Logs.Include(l => l.Group)
            .Include(l => l.Competition)
            .Where(l => l.UserId == request.UserId)
            .OrderByDescending(l => l.ActionTime)
            .AsNoTracking();

        // Get total count
        var total = await query.CountAsync(cancellationToken);

        // Apply pagination
        var logs = await query.Skip(request.Skip).Take(request.Take).ToListAsync(cancellationToken);

        var logDtos = logs.Select(l => new LogDto(
                l.Id,
                l.ActionType,
                l.ActionType.ToString(),
                l.ActionTime,
                l.IpAddress,
                l.UserId,
                user.Name,
                l.GroupId,
                l.Group?.Name,
                l.CompetitionId,
                l.Competition?.Name
            ))
            .ToList();

        _logger.LogInformation(
            "Retrieved {Count} logs for user {UserId}",
            logs.Count,
            request.UserId
        );

        return new GetUserLogsResult(logDtos, total, request.Skip, request.Take);
    }
}
