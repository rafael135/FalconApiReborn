using MediatR;

namespace Falcon.Api.Features.Competitions.GetAttempts;

public record GetAttemptsQuery(Guid CompetitionId, int Skip = 0, int Take = 10) : IRequest<GetAttemptsResult>;
