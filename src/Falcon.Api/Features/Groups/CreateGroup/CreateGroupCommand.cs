using MediatR;

namespace Falcon.Api.Features.Groups.CreateGroup;

/// <summary>
/// Command to create a new group.
/// </summary>
/// <param name="Name">The name of the group to create.</param>
public record CreateGroupCommand(string Name) : IRequest<CreateGroupResult>;
