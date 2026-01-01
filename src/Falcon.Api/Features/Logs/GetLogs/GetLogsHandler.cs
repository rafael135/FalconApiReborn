using Falcon.Api.Features.Logs.Shared;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Logs.GetLogs;

/// <summary>
/// Handler for getting system logs with filters.
/// </summary>
public class GetLogsHandler : IRequestHandler<GetLogsQuery, GetLogsResult>
{
    private readonly FalconDbContext _context;
    private readonly ILogger<GetLogsHandler> _logger;

    public GetLogsHandler(FalconDbContext context, ILogger<GetLogsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetLogsResult> Handle(GetLogsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Logs
            .Include(l => l.User)
            .Include(l => l.Group)
            .Include(l => l.Competition)
            .AsNoTracking();

        // Apply filters
        if (!string.IsNullOrEmpty(request.UserId))
        {
            query = query.Where(l => l.UserId == request.UserId);
        }

        if (request.GroupId.HasValue)
        {
            query = query.Where(l => l.GroupId == request.GroupId.Value);
        }

        if (request.CompetitionId.HasValue)
        {
            query = query.Where(l => l.CompetitionId == request.CompetitionId.Value);
        }

        if (request.ActionType.HasValue)
        {
            query = query.Where(l => (int)l.ActionType == request.ActionType.Value);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(l => l.ActionTime >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(l => l.ActionTime <= request.EndDate.Value);
        }

        // Order by action time descending (most recent first)
        query = query.OrderByDescending(l => l.ActionTime);

        // Get total count before pagination
        var total = await query.CountAsync(cancellationToken);

        // Apply pagination
        var logs = await query
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);

        var logDtos = logs.Select(l => new LogDto(
            l.Id,
            l.ActionType,
            l.ActionType.ToString(),
            l.ActionTime,
            l.IpAddress,
            l.UserId,
            l.User?.Name,
            l.GroupId,
            l.Group?.Name,
            l.CompetitionId,
            l.Competition?.Name
        )).ToList();

        _logger.LogInformation("Retrieved {Count} logs (total: {Total})", logs.Count, total);

        return new GetLogsResult(logDtos, total, request.Skip, request.Take);
    }
}
