using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.RemoveExercise;

public class RemoveExerciseHandler : IRequestHandler<RemoveExerciseCommand, RemoveExerciseResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<RemoveExerciseHandler> _logger;

    public RemoveExerciseHandler(FalconDbContext dbContext, ILogger<RemoveExerciseHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<RemoveExerciseResult> Handle(RemoveExerciseCommand request, CancellationToken cancellationToken)
    {
        var competition = await _dbContext.Competitions.FindAsync(new object[] { request.CompetitionId }, cancellationToken);
        if (competition == null)
            throw new NotFoundException("Competition", request.CompetitionId);

        if (competition.Status == CompetitionStatus.Ongoing || competition.Status == CompetitionStatus.Finished)
        {
            var errors = new Dictionary<string, string> { { "competition", "Não é possível remover exercícios de competição em andamento ou finalizada" } };
            throw new FormException(errors);
        }

        var exerciseInCompetition = await _dbContext.ExercisesInCompetition
            .FirstOrDefaultAsync(e => e.CompetitionId == request.CompetitionId && e.ExerciseId == request.ExerciseId, cancellationToken);

        if (exerciseInCompetition == null)
        {
            var errors = new Dictionary<string, string> { { "exercise", "Exercício não está na competição" } };
            throw new FormException(errors);
        }

        _dbContext.ExercisesInCompetition.Remove(exerciseInCompetition);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Exercise {ExerciseId} removed from competition {CompetitionId}", request.ExerciseId, request.CompetitionId);

        return new RemoveExerciseResult(true, "Exercício removido da competição com sucesso");
    }
}
