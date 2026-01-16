using MediatR;

namespace Falcon.Api.Features.Logs.GetUserLogs;

/// <summary>
/// Query to get logs for a specific user.
/// </summary>
public record GetUserLogsQuery(string UserId, int Skip = 0, int Take = 50)
    : IRequest<GetUserLogsResult>;
