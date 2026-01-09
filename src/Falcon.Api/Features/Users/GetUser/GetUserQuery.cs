using Falcon.Api.Features.Users.Shared;
using MediatR;

namespace Falcon.Api.Features.Users.GetUser;

/// <summary>
/// Query for retrieving detailed information about a specific user.
/// </summary>
/// <param name="UserId">The unique identifier of the user to retrieve.</param>
public record GetUserQuery(string UserId) : IRequest<GetUserResult>;
