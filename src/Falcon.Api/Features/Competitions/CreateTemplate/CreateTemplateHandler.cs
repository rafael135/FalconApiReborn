using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Falcon.Api.Features.Competitions.Shared;

namespace Falcon.Api.Features.Competitions.CreateTemplate;

/// <summary>
/// Handler for creating a competition template.
/// </summary>
public class CreateTemplateHandler : IRequestHandler<CreateTemplateCommand, CreateTemplateResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<CreateTemplateHandler> _logger;

    public CreateTemplateHandler(
        FalconDbContext dbContext,
        ILogger<CreateTemplateHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<CreateTemplateResult> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
    {
        // Validate inputs
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(nameof(request.Name), "Nome é obrigatório");
        else if (request.Name.Length > 200)
            errors.Add(nameof(request.Name), "Nome deve ter no máximo 200 caracteres");

        if (request.StartInscriptions >= request.EndInscriptions)
            errors.Add(nameof(request.EndInscriptions), "Data de término das inscrições deve ser posterior ao início");

        if (request.EndInscriptions >= request.StartTime)
            errors.Add(nameof(request.StartTime), "Data de início da competição deve ser posterior ao término das inscrições");

        if (errors.Any())
            throw new FormException(errors);

        // Create template using factory method
        var competition = Competition.CreateTemplate(
            request.Name,
            request.Description,
            request.StartInscriptions,
            request.EndInscriptions,
            request.StartTime
        );

        await _dbContext.Competitions.AddAsync(competition, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Competition template created: {CompetitionId} - {CompetitionName}", 
            competition.Id, competition.Name);

        var competitionDto = new CompetitionDto(
            competition.Id,
            competition.Name,
            competition.Description,
            competition.Status,
            competition.StartInscriptions,
            competition.EndInscriptions,
            competition.StartTime,
            competition.EndTime,
            competition.MaxMembers,
            competition.MaxExercises
        );

        return new CreateTemplateResult(competitionDto);
    }
}
