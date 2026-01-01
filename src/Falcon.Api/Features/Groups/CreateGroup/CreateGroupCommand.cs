using MediatR;

namespace Falcon.Api.Features.Groups.CreateGroup;

/// <summary>
/// Command to create a new group.
/// </summary>
/// <param name="Name">The name of the group to create.</param>
/// <param name="UserRAs">List of user RAs to add to the group.</param>
public record CreateGroupCommand(string Name, ICollection<string>? UserRAs = null) : IRequest<CreateGroupResult>;
