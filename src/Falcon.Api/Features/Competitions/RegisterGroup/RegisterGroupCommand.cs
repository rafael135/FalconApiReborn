using MediatR;

namespace Falcon.Api.Features.Competitions.RegisterGroup;

/// <summary>
/// Command to register the current user's group in a competition.
/// </summary>
public record RegisterGroupCommand(Guid CompetitionId) : IRequest<RegisterGroupResult>;
