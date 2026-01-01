using MediatR;

namespace Falcon.Api.Features.Exercises.GetExercise;

/// <summary>
/// Query to get an exercise by ID.
/// </summary>
public record GetExerciseQuery(Guid ExerciseId) : IRequest<GetExerciseResult>;
