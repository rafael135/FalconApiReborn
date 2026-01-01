using MediatR;

namespace Falcon.Api.Features.Competitions.StartCompetition;

/// <summary>
/// Command to start a competition.
/// </summary>
public record StartCompetitionCommand(Guid CompetitionId) : IRequest<StartCompetitionResult>;
