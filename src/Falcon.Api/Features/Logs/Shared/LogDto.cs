using Falcon.Core.Domain.Shared.Enums;

namespace Falcon.Api.Features.Logs.Shared;

/// <summary>
/// Log entry information.
/// </summary>
public record LogDto(
    Guid Id,
    LogType ActionType,
    string ActionTypeName,
    DateTime ActionTime,
    string IpAddress,
    string? UserId,
    string? UserName,
    Guid? GroupId,
    string? GroupName,
    Guid? CompetitionId,
    string? CompetitionTitle
);
