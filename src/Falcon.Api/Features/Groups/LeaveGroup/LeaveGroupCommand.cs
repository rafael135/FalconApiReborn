using MediatR;

namespace Falcon.Api.Features.Groups.LeaveGroup;

/// <summary>
/// Command for a user to leave their current group.
/// </summary>
public record LeaveGroupCommand : IRequest<LeaveGroupResult>;
