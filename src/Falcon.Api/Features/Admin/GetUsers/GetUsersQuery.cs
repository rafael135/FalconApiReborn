using MediatR;

namespace Falcon.Api.Features.Admin.GetUsers;

/// <summary>
/// Query to get all users with filtering and pagination.
/// </summary>
/// <param name="Role">An optional property of type string to filter users by role.</param>
/// <param name="Search">An optional property of type string to search users by name, email, or RA.</param>
/// <param name="Skip">The number of records to skip for pagination.</param>
/// <param name="Take">The number of records to take for pagination.</param>
public record GetUsersQuery(string? Role, string? Search, int Skip = 0, int Take = 50)
    : IRequest<GetUsersResult>;
