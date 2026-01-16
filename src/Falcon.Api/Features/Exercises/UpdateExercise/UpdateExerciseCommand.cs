using MediatR;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

/// <summary>
/// Comando para atualizar um exercício. Contém o ID do exercício, os metadados atualizados e um arquivo opcional.
/// </summary>
public record UpdateExerciseCommand(
    Guid ExerciseId,
    UpdateExerciseRequestDto Metadata,
    IFormFile? File
) : IRequest<UpdateExerciseResult>;
