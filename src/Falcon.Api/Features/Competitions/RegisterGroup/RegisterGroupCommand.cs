using MediatR;

namespace Falcon.Api.Features.Competitions.RegisterGroup;

/// <summary>
/// Command to register the current user's group in a competition.
/// </summary>
/// <param name="CompetitionId">The competition identifier to register in.</param>
public record RegisterGroupCommand(Guid CompetitionId) : IRequest<RegisterGroupResult>;
