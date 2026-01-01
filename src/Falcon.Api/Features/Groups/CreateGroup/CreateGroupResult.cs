using Falcon.Api.Features.Groups.Shared;

namespace Falcon.Api.Features.Groups.CreateGroup;

/// <summary>
/// Result of creating a group.
/// </summary>
public record CreateGroupResult(GroupDto Group);
