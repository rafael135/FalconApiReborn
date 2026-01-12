using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.PromoteTemplate;

/// <summary>
/// Result of promoting a template to an active competition.
/// </summary>
/// <param name="Competition">The created competition DTO after promotion.</param>
public record PromoteTemplateResult(CompetitionDto Competition); 
