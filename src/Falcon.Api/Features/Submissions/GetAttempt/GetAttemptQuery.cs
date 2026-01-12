using MediatR;

namespace Falcon.Api.Features.Submissions.GetAttempt;

/// <summary>
/// Query to fetch a specific attempt by id.
/// </summary>
/// <param name="AttemptId">Attempt unique identifier.</param>
public record GetAttemptQuery(Guid AttemptId) : IRequest<GetAttemptResult>;
