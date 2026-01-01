using MediatR;

namespace Falcon.Api.Features.Logs.GetLogs;

/// <summary>
/// Query to get system logs with filters.
/// </summary>
public record GetLogsQuery(
    string? UserId,
    Guid? GroupId,
    Guid? CompetitionId,
    int? ActionType,
    DateTime? StartDate,
    DateTime? EndDate,
    int Skip = 0,
    int Take = 50
) : IRequest<GetLogsResult>;
