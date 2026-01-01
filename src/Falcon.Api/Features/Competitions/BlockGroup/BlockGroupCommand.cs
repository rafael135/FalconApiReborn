using MediatR;

namespace Falcon.Api.Features.Competitions.BlockGroup;

/// <summary>
/// Command to block a group from submitting in a competition.
/// </summary>
public record BlockGroupCommand(Guid CompetitionId, Guid GroupId) : IRequest<BlockGroupResult>;
