using MediatR;

namespace Falcon.Api.Features.Submissions.GetAttempt;

public record GetAttemptQuery(Guid AttemptId) : IRequest<GetAttemptResult>;
