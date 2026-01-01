using Falcon.Api.Features.Groups.Shared;

namespace Falcon.Api.Features.Groups.GetGroup;

/// <summary>
/// Result of retrieving a group.
/// </summary>
public record GetGroupResult(GroupDetailDto Group);
