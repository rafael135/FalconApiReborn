using MediatR;

namespace Falcon.Api.Features.Competitions.PromoteTemplate;

/// <summary>
/// Command to promote a template to an active competition.
/// </summary>
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
