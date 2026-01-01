using Falcon.Api.Features.Exercises.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

public class UpdateExerciseHandler : IRequestHandler<UpdateExerciseCommand, UpdateExerciseResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly ILogger<UpdateExerciseHandler> _logger;

    public UpdateExerciseHandler(FalconDbContext dbContext, ILogger<UpdateExerciseHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<UpdateExerciseResult> Handle(UpdateExerciseCommand request, CancellationToken cancellationToken)
    {
        var exercise = await _dbContext.Exercises
            .Include(e => e.ExerciseType)
            .FirstOrDefaultAsync(e => e.Id == request.ExerciseId, cancellationToken);

        if (exercise == null)
            throw new NotFoundException("Exercise", request.ExerciseId);

        if (request.Title != null || request.Description != null || request.ExerciseTypeId != null)
        {
            var title = request.Title ?? exercise.Title;
            var description = request.Description ?? exercise.Description;
            var typeId = request.ExerciseTypeId ?? exercise.ExerciseTypeId;

            if (request.ExerciseTypeId != null)
            {
                var typeExists = await _dbContext.ExerciseTypes.AnyAsync(et => et.Id == typeId, cancellationToken);
                if (!typeExists)
                {
                    var errors = new Dictionary<string, string> { { "exerciseTypeId", "Tipo de exercício não encontrado" } };
                    throw new FormException(errors);
                }
            }

            exercise.UpdateDetauls(title, description, typeId);
            await _dbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Exercise {ExerciseId} updated", request.ExerciseId);
        }

        var exerciseDto = new ExerciseDto(
            exercise.Id, exercise.Title, exercise.Description, exercise.EstimatedTime,
            exercise.ExerciseTypeId, exercise.ExerciseType.Label, exercise.AttachedFileId
        );

        return new UpdateExerciseResult(exerciseDto);
    }
}
