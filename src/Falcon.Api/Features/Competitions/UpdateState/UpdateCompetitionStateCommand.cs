using MediatR;

namespace Falcon.Api.Features.Competitions.UpdateState;

/// <summary>
/// Command for updating the state of a competition.
/// </summary>
public record UpdateCompetitionStateCommand : IRequest;
