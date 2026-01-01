using MediatR;

namespace Falcon.Api.Features.Submissions.GetGroupAttempts;

public record GetGroupAttemptsQuery(Guid? CompetitionId, Guid? ExerciseId) : IRequest<GetGroupAttemptsResult>;
