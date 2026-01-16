using MediatR;

namespace Falcon.Api.Features.Users.GetCompetitionHistory;

/// <summary>
/// Query for retrieving a user's competition participation history.
/// </summary>
/// <param name="UserId">The unique identifier of the user.</param>
/// <param name="Skip">The number of records to skip for pagination.</param>
/// <param name="Take">The number of records to take for pagination.</param>
public record GetCompetitionHistoryQuery(string UserId, int Skip = 0, int Take = 10)
    : IRequest<GetCompetitionHistoryResult>;
