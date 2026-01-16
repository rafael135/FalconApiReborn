using MediatR;

namespace Falcon.Api.Features.Competitions.BlockGroup;

/// <summary>
/// Command to block a group from submitting in a competition.
/// </summary>
/// <param name="CompetitionId">The competition identifier.</param>
/// <param name="GroupId">The group identifier to block.</param>
public record BlockGroupCommand(Guid CompetitionId, Guid GroupId) : IRequest<BlockGroupResult>;
