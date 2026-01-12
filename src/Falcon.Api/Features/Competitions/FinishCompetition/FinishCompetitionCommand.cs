using MediatR;

namespace Falcon.Api.Features.Competitions.FinishCompetition;

/// <summary>
/// Command to finish a competition.
/// </summary>
/// <param name="CompetitionId">The competition identifier to finish.</param>
public record FinishCompetitionCommand(Guid CompetitionId) : IRequest<FinishCompetitionResult>; 
