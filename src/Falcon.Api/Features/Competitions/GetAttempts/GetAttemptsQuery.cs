using MediatR;

namespace Falcon.Api.Features.Competitions.GetAttempts;

/// <summary>
/// Query to get attempts for a competition with pagination.
/// </summary>
/// <param name="CompetitionId">The competition identifier.</param>
/// <param name="Skip">Number of items to skip (pagination).</param>
/// <param name="Take">Number of items to take (pagination).</param>
public record GetAttemptsQuery(Guid CompetitionId, int Skip = 0, int Take = 10) : IRequest<GetAttemptsResult>; 
