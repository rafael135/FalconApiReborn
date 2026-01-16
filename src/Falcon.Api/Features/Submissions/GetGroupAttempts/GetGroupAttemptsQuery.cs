using MediatR;

namespace Falcon.Api.Features.Submissions.GetGroupAttempts;

/// <summary>
/// Query to retrieve attempts for the authenticated user's group with optional filters.
/// </summary>
/// <param name="CompetitionId">Optional competition id to filter attempts.</param>
/// <param name="ExerciseId">Optional exercise id to filter attempts.</param>
public record GetGroupAttemptsQuery(Guid? CompetitionId, Guid? ExerciseId)
    : IRequest<GetGroupAttemptsResult>;
