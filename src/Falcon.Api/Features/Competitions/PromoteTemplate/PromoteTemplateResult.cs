using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.PromoteTemplate;

/// <summary>
/// Result of promoting a template to an active competition.
/// </summary>
public record PromoteTemplateResult(CompetitionDto Competition);
