using MediatR;

namespace Falcon.Api.Features.Exercises.GetExercises;

/// <summary>
/// Query to get exercises with optional filtering and pagination.
/// </summary>
public record GetExercisesQuery(int? ExerciseTypeId = null, int Skip = 0, int Take = 10)
    : IRequest<GetExercisesResult>;
