using Falcon.Api.Features.Exercises.Shared;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Interfaces;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Exercises.CreateExercise;

/// <summary>
/// Handler for creating a new exercise.
/// </summary>
public class CreateExerciseHandler : IRequestHandler<CreateExerciseCommand, CreateExerciseResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly IJudgeService _judgeService;
    private readonly IAttachedFileService _attachedFileService;
    private readonly ILogger<CreateExerciseHandler> _logger;

    public CreateExerciseHandler(
        FalconDbContext dbContext,
        IJudgeService judgeService,
        IAttachedFileService attachedFileService,
        ILogger<CreateExerciseHandler> logger)
    {
        _dbContext = dbContext;
        _judgeService = judgeService;
        _attachedFileService = attachedFileService;
        _logger = logger;
    }

    public async Task<CreateExerciseResult> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
    {
        var metadata = request.Metadata;

        // Validate inputs
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(metadata.Title))
            errors.Add(nameof(metadata.Title), "Título é obrigatório");
        else if (metadata.Title.Length > 200)
            errors.Add(nameof(metadata.Title), "Título deve ter no máximo 200 caracteres");

        if (metadata.EstimatedTime <= TimeSpan.Zero)
            errors.Add(nameof(metadata.EstimatedTime), "Tempo estimado deve ser maior que zero");

        if (errors.Any())
            throw new FormException(errors);

        // Verify ExerciseType exists
        var exerciseType = await _dbContext.ExerciseTypes
            .FirstOrDefaultAsync(et => et.Id == metadata.ExerciseTypeId, cancellationToken);

        if (exerciseType == null)
        {
            var typeErrors = new Dictionary<string, string>
            {
                { nameof(metadata.ExerciseTypeId), "Tipo de exercício não encontrado" }
            };
            throw new FormException(typeErrors);
        }

        // Create Exercise entity
        var exercise = new Exercise(
            metadata.Title,
            metadata.Description,
            metadata.ExerciseTypeId,
            metadata.EstimatedTime
        );

        // Handle File
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

        // Handle Test Cases
        var inputs = metadata.Inputs.OrderBy(i => i.OrderId).ToList();
        var outputs = metadata.Outputs.OrderBy(o => o.OrderId).ToList();
        var judgeTestCases = new List<TestCase>();

        foreach (var inputDto in inputs)
        {
            var outputDto = outputs.FirstOrDefault(o => o.OrderId == inputDto.OrderId);
            if (outputDto != null)
            {
                exercise.AddTestCase(inputDto.Input, outputDto.Output);
                judgeTestCases.Add(new TestCase(inputDto.Input, outputDto.Output));
            }
        }

        // Create exercise in Judge system
        string? judgeUuid = await _judgeService.CreateExerciseAsync(
            metadata.Title,
            metadata.Description ?? "",
            judgeTestCases
        );

        exercise.SetJudgeUuid(judgeUuid);

        await _dbContext.Exercises.AddAsync(exercise, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Exercise {ExerciseId} created with Judge UUID {JudgeUuid}", 
            exercise.Id, judgeUuid);

        var exerciseDto = new ExerciseDto(
            exercise.Id,
            exercise.Title,
            exercise.Description,
            exercise.EstimatedTime,
            exercise.ExerciseTypeId,
            exerciseType.Label,
            exercise.AttachedFileId
        );

        return new CreateExerciseResult(exerciseDto);
    }
}
