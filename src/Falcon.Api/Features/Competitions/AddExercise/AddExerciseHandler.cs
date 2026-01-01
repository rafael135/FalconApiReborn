using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Competitions.AddExercise;

public class AddExerciseHandler : IRequestHandler<AddExerciseCommand, AddExerciseResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<AddExerciseHandler> _logger;

    public AddExerciseHandler(FalconDbContext dbContext, ILogger<AddExerciseHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<AddExerciseResult> Handle(AddExerciseCommand request, CancellationToken cancellationToken)
    {
        var competition = await _dbContext.Competitions
            .Include(c => c.ExercisesInCompetition)
            .FirstOrDefaultAsync(c => c.Id == request.CompetitionId, cancellationToken);

        if (competition == null)
            throw new NotFoundException("Competition", request.CompetitionId);

        if (competition.Status == CompetitionStatus.Finished)
        {
            var errors = new Dictionary<string, string> { { "competition", "Não é possível adicionar exercícios a uma competição finalizada" } };
            throw new FormException(errors);
        }

        var exercise = await _dbContext.Exercises.FindAsync(new object[] { request.ExerciseId }, cancellationToken);
        if (exercise == null)
            throw new NotFoundException("Exercise", request.ExerciseId);

        if (competition.ExercisesInCompetition.Any(e => e.ExerciseId == request.ExerciseId))
        {
            var errors = new Dictionary<string, string> { { "exercise", "Exercício já está na competição" } };
            throw new FormException(errors);
        }

        if (competition.MaxExercises.HasValue && competition.ExercisesInCompetition.Count >= competition.MaxExercises.Value)
        {
            var errors = new Dictionary<string, string> { { "competition", $"Competição já atingiu o máximo de {competition.MaxExercises} exercícios" } };
            throw new FormException(errors);
        }

        competition.AddExercise(exercise);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Exercise {ExerciseId} added to competition {CompetitionId}", request.ExerciseId, request.CompetitionId);

        return new AddExerciseResult(true, "Exercício adicionado à competição com sucesso");
    }
}
