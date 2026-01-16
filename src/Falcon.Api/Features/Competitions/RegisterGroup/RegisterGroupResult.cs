using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.RegisterGroup;

/// <summary>
/// Result of registering a group in a competition.
/// </summary>
/// <param name="Registration">Details of the created group registration.</param>
public record RegisterGroupResult(GroupInCompetitionDto Registration);
