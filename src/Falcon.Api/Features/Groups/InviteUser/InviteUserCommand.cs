using MediatR;

namespace Falcon.Api.Features.Groups.InviteUser;

/// <summary>
/// Command to invite a user to a group.
/// </summary>
/// <param name="GroupId">The unique identifier of the group.</param>
/// <param name="RA">The RA (Student ID) of the user to invite.</param>
public record InviteUserCommand(Guid GroupId, string RA) : IRequest<InviteUserResult>;
