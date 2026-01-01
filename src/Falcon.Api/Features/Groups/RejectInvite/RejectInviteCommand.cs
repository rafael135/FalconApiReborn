using MediatR;

namespace Falcon.Api.Features.Groups.RejectInvite;

/// <summary>
/// Command to reject a group invitation.
/// </summary>
/// <param name="InviteId">The unique identifier of the invitation.</param>
public record RejectInviteCommand(Guid InviteId) : IRequest<RejectInviteResult>;
