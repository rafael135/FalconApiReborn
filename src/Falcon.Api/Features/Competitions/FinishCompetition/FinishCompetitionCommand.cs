using MediatR;

namespace Falcon.Api.Features.Competitions.FinishCompetition;

/// <summary>
/// Command to finish a competition.
/// </summary>
public record FinishCompetitionCommand(Guid CompetitionId) : IRequest<FinishCompetitionResult>;
