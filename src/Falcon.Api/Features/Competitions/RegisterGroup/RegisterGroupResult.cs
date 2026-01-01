using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.RegisterGroup;

/// <summary>
/// Result of registering a group in a competition.
/// </summary>
public record RegisterGroupResult(GroupInCompetitionDto Registration);
