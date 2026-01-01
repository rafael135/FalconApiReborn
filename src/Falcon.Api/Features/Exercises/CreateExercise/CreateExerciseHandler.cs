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
    private readonly ILogger<CreateExerciseHandler> _logger;

    public CreateExerciseHandler(
        FalconDbContext dbContext,
        IJudgeService judgeService,
        ILogger<CreateExerciseHandler> logger)
    {
        _dbContext = dbContext;
        _judgeService = judgeService;
        _logger = logger;
    }

    public async Task<CreateExerciseResult> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
    {
        // Validate inputs
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(request.Title))
            errors.Add(nameof(request.Title), "Título é obrigatório");
        else if (request.Title.Length > 200)
            errors.Add(nameof(request.Title), "Título deve ter no máximo 200 caracteres");

        if (request.EstimatedTime <= TimeSpan.Zero)
            errors.Add(nameof(request.EstimatedTime), "Tempo estimado deve ser maior que zero");

        if (errors.Any())
            throw new FormException(errors);

        // Verify ExerciseType exists
        var exerciseType = await _dbContext.ExerciseTypes
            .FirstOrDefaultAsync(et => et.Id == request.ExerciseTypeId, cancellationToken);

        if (exerciseType == null)
        {
            var typeErrors = new Dictionary<string, string>
            {
                { nameof(request.ExerciseTypeId), "Tipo de exercício não encontrado" }
            };
            throw new FormException(typeErrors);
        }

        // Create Exercise entity
        var exercise = new Exercise(
            request.Title,
            request.Description,
            request.ExerciseTypeId,
            request.EstimatedTime
        );

        // Create exercise in Judge system
        var judgeUuid = await _judgeService.CreateExerciseAsync(
            request.Title,
            request.Description ?? "",
            new List<TestCase>() // No test cases initially
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
