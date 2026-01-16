using Falcon.Api.Features.Exercises.Shared;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Files;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

public class UpdateExerciseHandler : IRequestHandler<UpdateExerciseCommand, UpdateExerciseResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly IAttachedFileService _attachedFileService;
    private readonly ILogger<UpdateExerciseHandler> _logger;

    public UpdateExerciseHandler(
        FalconDbContext dbContext,
        IAttachedFileService attachedFileService,
        ILogger<UpdateExerciseHandler> logger
    )
    {
        _dbContext = dbContext;
        _attachedFileService = attachedFileService;
        _logger = logger;
    }

    public async Task<UpdateExerciseResult> Handle(
        UpdateExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        var exercise = await _dbContext
            .Exercises.Include(e => e.ExerciseType)
            .Include(e => e.Inputs)
            .Include(e => e.Outputs)
            .FirstOrDefaultAsync(e => e.Id == request.ExerciseId, cancellationToken);

        if (exercise == null)
            throw new NotFoundException("Exercise", request.ExerciseId);

        var metadata = request.Metadata;

        // Validate Exercise Type
        var typeExists = await _dbContext.ExerciseTypes.AnyAsync(
            et => et.Id == metadata.ExerciseTypeId,
            cancellationToken
        );
        if (!typeExists)
        {
            var errors = new Dictionary<string, string>
            {
                { "exerciseTypeId", "Tipo de exercício não encontrado" },
            };
            throw new FormException(errors);
        }

        exercise.UpdateDetails(metadata.Title, metadata.Description, metadata.ExerciseTypeId);

        // Update File
        if (request.File != null)
        {
            var file = request.File;
            var attachedFile = await _attachedFileService.CreateAttachedFileAsync(
                file.OpenReadStream(),
                file.FileName,
                file.ContentType,
                file.Length,
                cancellationToken
            );

            exercise.SetAttachedFile(attachedFile);
        }

        // Update Test Cases (Full Replacement Strategy)
        _dbContext.ExerciseInputs.RemoveRange(exercise.Inputs);
        _dbContext.ExerciseOutputs.RemoveRange(exercise.Outputs);

        exercise.ClearTestCases();

        var inputs = metadata.Inputs.OrderBy(i => i.OrderId).ToList();
        var outputs = metadata.Outputs.OrderBy(o => o.OrderId).ToList();

        foreach (var inputDto in inputs)
        {
            var outputDto = outputs.FirstOrDefault(o => o.OrderId == inputDto.OrderId);
            if (outputDto != null)
            {
                exercise.AddTestCase(inputDto.Input, outputDto.Output);
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Exercise {ExerciseId} updated", request.ExerciseId);

        var exerciseDto = new ExerciseDto(
            exercise.Id,
            exercise.Title,
            exercise.Description,
            exercise.EstimatedTime,
            exercise.ExerciseTypeId,
            exercise.ExerciseType.Label,
            exercise.AttachedFileId
        );

        return new UpdateExerciseResult(exerciseDto);
    }
}
