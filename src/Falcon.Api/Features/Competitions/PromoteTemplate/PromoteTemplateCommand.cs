using MediatR;

namespace Falcon.Api.Features.Competitions.PromoteTemplate;

/// <summary>
/// Command to promote a template to an active competition.
/// </summary>
/// <param name="TemplateId">The template competition identifier.</param>
/// <param name="MaxMembers">Maximum members per group.</param>
/// <param name="MaxExercises">Maximum exercises in the competition.</param>
/// <param name="MaxSubmissionSize">Maximum submission size (bytes).</param>
/// <param name="Duration">Competition duration as a TimeSpan (e.g., "01:00:00").</param>
/// <param name="StopRanking">Time before the end when ranking stops (TimeSpan).</param>
/// <param name="BlockSubmissions">Time window used to block late submissions (TimeSpan).</param>
/// <param name="Penalty">Penalty duration applied to late submissions (TimeSpan).</param>
public record PromoteTemplateCommand(
    Guid TemplateId,
    int MaxMembers,
    int MaxExercises,
    int MaxSubmissionSize,
    TimeSpan Duration,
    TimeSpan StopRanking,
    TimeSpan BlockSubmissions,
    TimeSpan Penalty
) : IRequest<PromoteTemplateResult>;
