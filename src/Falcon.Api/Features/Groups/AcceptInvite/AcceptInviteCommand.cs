using MediatR;

namespace Falcon.Api.Features.Groups.AcceptInvite;

/// <summary>
/// Command to accept a group invitation.
/// </summary>
/// <param name="InviteId">The unique identifier of the invitation.</param>
public record AcceptInviteCommand(Guid InviteId) : IRequest<AcceptInviteResult>;
