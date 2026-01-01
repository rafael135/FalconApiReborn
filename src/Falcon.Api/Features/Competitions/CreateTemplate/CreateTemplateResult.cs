using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.CreateTemplate;

/// <summary>
/// Result of creating a competition template.
/// </summary>
public record CreateTemplateResult(CompetitionDto Competition);
