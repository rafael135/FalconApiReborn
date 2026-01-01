using MediatR;

namespace Falcon.Api.Features.Competitions.UnregisterGroup;

/// <summary>
/// Command to unregister the current user's group from a competition.
/// </summary>
public record UnregisterGroupCommand(Guid CompetitionId) : IRequest<UnregisterGroupResult>;
