using MediatR;

namespace Falcon.Api.Features.Groups.GetGroup;

/// <summary>
/// Query to retrieve a group by its ID.
/// </summary>
/// <param name="GroupId">The unique identifier of the group.</param>
public record GetGroupQuery(Guid GroupId) : IRequest<GetGroupResult>;
