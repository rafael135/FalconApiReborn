using MediatR;

namespace Falcon.Api.Features.Competitions.UnregisterGroup;

/// <summary>
/// Command to unregister the current user's group from a competition.
/// </summary>
/// <param name="CompetitionId">The competition identifier to cancel registration for.</param>
public record UnregisterGroupCommand(Guid CompetitionId) : IRequest<UnregisterGroupResult>; 
