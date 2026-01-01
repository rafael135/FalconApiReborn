using MediatR;

namespace Falcon.Api.Features.Admin.GetUsers;

/// <summary>
/// Query to get all users with filtering and pagination.
/// </summary>
public record GetUsersQuery(
    string? Role,
    int Skip = 0,
    int Take = 50
) : IRequest<GetUsersResult>;
