using Falcon.Api.Features.Logs.Shared;

namespace Falcon.Api.Features.Logs.GetLogs;

/// <summary>
/// Result containing paginated logs.
/// </summary>
public record GetLogsResult(List<LogDto> Logs, int Total, int Skip, int Take);
