using Falcon.Api.Features.Exercises.Shared;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Infrastructure.Database;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.Features.Exercises.GetExercise;

/// <summary>
/// Handler for getting an exercise by ID.
/// </summary>
/// <remarks>
/// Includes test cases only when the current user has the Teacher or Admin role. The handler maps
/// attached file metadata and test cases (inputs/outputs) to <see cref="ExerciseDetailDto"/>.
/// </remarks>
/// <exception cref="Falcon.Core.Domain.Shared.Exceptions.NotFoundException">Thrown when the exercise does not exist.</exception>
public class GetExerciseHandler : IRequestHandler<GetExerciseQuery, GetExerciseResult>
{
    private readonly FalconDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetExerciseHandler(
        FalconDbContext dbContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<GetExerciseResult> Handle(GetExerciseQuery request, CancellationToken cancellationToken)
    {
        // Get current user role to determine if test cases should be included
        var httpContext = _httpContextAccessor.HttpContext;
        var isTeacherOrAdmin = httpContext?.User.IsInRole("Teacher") == true || 
                               httpContext?.User.IsInRole("Admin") == true;

        IQueryable<Core.Domain.Exercises.Exercise> query = _dbContext.Exercises
            .AsNoTracking()
            .Include(e => e.ExerciseType)
            .Include(e => e.AttachedFile);

        // Include test cases only for Teachers/Admins
        if (isTeacherOrAdmin)
        {
            query = query
                .Include(e => e.Inputs)
                .Include(e => e.Outputs);
        }

        var exercise = await query
            .FirstOrDefaultAsync(e => e.Id == request.ExerciseId, cancellationToken);

        if (exercise == null)
        {
            throw new NotFoundException("Exercise", request.ExerciseId);
        }

        // Map test cases if user is Teacher/Admin
        List<TestCaseDto>? testCases = null;
        if (isTeacherOrAdmin && exercise.Inputs.Any())
        {
            testCases = exercise.Inputs
                .Select(input => new TestCaseDto(
                    input.Id,
                    exercise.Outputs.First(o => o.ExerciseInputId == input.Id).Id,
                    input.InputContent,
                    exercise.Outputs.First(o => o.ExerciseInputId == input.Id).OutputContent
                ))
                .ToList();
        }

        // Map attached file if exists
        AttachedFileDto? attachedFileDto = null;
        if (exercise.AttachedFile != null)
        {
            attachedFileDto = new AttachedFileDto(
                exercise.AttachedFile.Id,
                exercise.AttachedFile.Name,
                exercise.AttachedFile.Type,
                exercise.AttachedFile.Size,
                exercise.AttachedFile.CreatedAt
            );
        }

        var exerciseDetailDto = new ExerciseDetailDto(
            exercise.Id,
            exercise.Title,
            exercise.Description,
            exercise.EstimatedTime,
            exercise.ExerciseType.Label,
            attachedFileDto,
            testCases
        );

        return new GetExerciseResult(exerciseDetailDto);
    }
}
