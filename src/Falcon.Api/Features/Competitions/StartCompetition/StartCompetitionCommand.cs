using MediatR;

namespace Falcon.Api.Features.Competitions.StartCompetition;

/// <summary>
/// Command to start a competition.
/// </summary>
/// <param name="CompetitionId">The competition identifier to start.</param>
public record StartCompetitionCommand(Guid CompetitionId) : IRequest<StartCompetitionResult>;
