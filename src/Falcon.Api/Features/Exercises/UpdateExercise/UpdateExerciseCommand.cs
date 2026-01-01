using MediatR;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

public record UpdateExerciseCommand(
    Guid ExerciseId,
    UpdateExerciseRequestDto Metadata,
    IFormFile? File
) : IRequest<UpdateExerciseResult>;
