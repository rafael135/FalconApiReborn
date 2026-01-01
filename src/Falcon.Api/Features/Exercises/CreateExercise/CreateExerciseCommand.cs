using MediatR;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Exercises.CreateExercise;

/// <summary>
/// Command to create a new exercise.
/// </summary>
public record CreateExerciseCommand(
    CreateExerciseRequestDto Metadata,
    IFormFile? File
) : IRequest<CreateExerciseResult>;
