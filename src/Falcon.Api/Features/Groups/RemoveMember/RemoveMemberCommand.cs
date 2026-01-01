using MediatR;

namespace Falcon.Api.Features.Groups.RemoveMember;

/// <summary>
/// Command to remove a member from a group.
/// </summary>
/// <param name="GroupId">The unique identifier of the group.</param>
/// <param name="UserId">The unique identifier of the user to remove.</param>
public record RemoveMemberCommand(Guid GroupId, string UserId) : IRequest<RemoveMemberResult>;
