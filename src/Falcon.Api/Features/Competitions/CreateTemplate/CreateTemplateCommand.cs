using MediatR;

namespace Falcon.Api.Features.Competitions.CreateTemplate;

/// <summary>
/// Command to create a competition template.
/// </summary>
public record CreateTemplateCommand(
    string Name,
    string Description,
    DateTime StartInscriptions,
    DateTime EndInscriptions,
    DateTime StartTime
) : IRequest<CreateTemplateResult>;
