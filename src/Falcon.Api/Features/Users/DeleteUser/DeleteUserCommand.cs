using MediatR;

namespace Falcon.Api.Features.Users.DeleteUser;

/// <summary>
/// Command for soft deleting a user.
/// </summary>
/// <param name="UserId">The unique identifier of the user to delete.</param>
public record DeleteUserCommand(string UserId) : IRequest<DeleteUserResult>;
