using Falcon.Api.Features.Competitions.Shared;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.PromoteTemplate;

/// <summary>
/// Handler for promoting a competition template to an active competition.
/// </summary>
public class PromoteTemplateHandler : IRequestHandler<PromoteTemplateCommand, PromoteTemplateResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<PromoteTemplateHandler> _logger;

    public PromoteTemplateHandler(FalconDbContext dbContext, ILogger<PromoteTemplateHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Handles the <see cref="PromoteTemplateCommand"/> and promotes a template to an active competition.
    /// </summary>
    /// <param name="request">Promotion parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created competition representation after promotion.</returns>
    /// <exception cref="FormException">Thrown for invalid input parameters or if the template is not promotable.</exception>
    /// <exception cref="NotFoundException">Thrown when the template competition is not found.</exception>
    public async Task<PromoteTemplateResult> Handle(
        PromoteTemplateCommand request,
        CancellationToken cancellationToken
    )
    {
        // Validate inputs
        var errors = new Dictionary<string, string>();

        if (request.MaxMembers < 1)
            errors.Add(nameof(request.MaxMembers), "Número máximo de membros deve ser maior que 0");

        if (request.MaxExercises < 1)
            errors.Add(
                nameof(request.MaxExercises),
                "Número máximo de exercícios deve ser maior que 0"
            );

        if (request.MaxSubmissionSize < 1)
            errors.Add(
                nameof(request.MaxSubmissionSize),
                "Tamanho máximo de submissão deve ser maior que 0"
            );

        if (errors.Any())
            throw new FormException(errors);

        // Get template competition
        var template = await _dbContext.Competitions.FirstOrDefaultAsync(
            c => c.Id == request.TemplateId,
            cancellationToken
        );

        if (template == null)
        {
            throw new NotFoundException("Competition", request.TemplateId);
        }

        // Verify it's a template
        if (template.Status != CompetitionStatus.ModelTemplate)
        {
            var validationErrors = new Dictionary<string, string>
            {
                { "templateId", "Competição não é um template" },
            };
            throw new FormException(validationErrors);
        }

        // Call domain method to promote
        template.PromoteToCompetition(
            request.MaxMembers,
            request.MaxExercises,
            request.MaxSubmissionSize,
            request.Duration,
            request.StopRanking,
            request.BlockSubmissions,
            request.Penalty
        );

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Competition template {TemplateId} promoted to active competition",
            request.TemplateId
        );

        var competitionDto = new CompetitionDto(
            template.Id,
            template.Name,
            template.Description,
            template.Status,
            template.StartInscriptions,
            template.EndInscriptions,
            template.StartTime,
            template.EndTime,
            template.MaxMembers,
            template.MaxExercises
        );

        return new PromoteTemplateResult(competitionDto);
    }
}
