using MediatR;

namespace Falcon.Api.Features.Groups.UpdateGroup;

/// <summary>
/// Command to update a group's name.
/// </summary>
/// <param name="GroupId">The unique identifier of the group to update.</param>
/// <param name="Name">The new name for the group.</param>
/// <param name="MembersToRemove">List of user RAs to remove from the group.</param>
public record UpdateGroupCommand(
    Guid GroupId,
    string Name,
    ICollection<string>? MembersToRemove = null
) : IRequest<UpdateGroupResult>;
