using System.Text.Json;
using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

/// <summary>
/// Atualiza um exercício existente. Endpoint que aceita formulário multipart/form-data.
/// </summary>
/// <remarks>
/// Este endpoint espera um formulário multipart onde:
/// - `metadata` é um campo string contendo o JSON serializado de <see cref="UpdateExerciseRequestDto"/>;
/// - `file` é um arquivo opcional anexado.
/// Exemplo de uso via curl:
/// <code>
/// curl -X PUT "https://localhost:5001/api/Exercise/00000000-0000-0000-0000-000000000000" \
///  -F "metadata={ \"id\": \"00000000-0000-0000-0000-000000000000\", \"title\": \"Novo Título\", \"exerciseTypeId\":1 }" \
///  -F "file=@exercise.pdf"
/// </code>
/// </remarks>
public class UpdateExerciseEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("api/Exercise/{id}", [Authorize(Roles = "Teacher,Admin")] async (
            IMediator mediator, 
            Guid id, 
            IFormFile? file,
            [FromForm(Name = "metadata")] string metadataJson) =>
        {
            if (string.IsNullOrEmpty(metadataJson))
            {
                return Results.BadRequest("Metadata is required");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var metadata = JsonSerializer.Deserialize<UpdateExerciseRequestDto>(metadataJson, options);
            
            if (metadata == null)
            {
                return Results.BadRequest("Invalid metadata format");
            }

            if (metadata.Id != id) 
            {
                return Results.BadRequest(new { error = "Route ID does not match command ExerciseId" });
            }

            var command = new UpdateExerciseCommand(id, metadata, file);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("UpdateExercise")
        .WithTags("Exercises")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<UpdateExerciseResult>()
        .DisableAntiforgery();
    }
} 
