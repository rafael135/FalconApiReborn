using Falcon.Api.Features.Logs.Shared;

namespace Falcon.Api.Features.Logs.GetUserLogs;

/// <summary>
/// Result containing user's logs.
/// </summary>
public record GetUserLogsResult(List<LogDto> Logs, int Total, int Skip, int Take);
