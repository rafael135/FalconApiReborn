using System.Text.Json;
using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.CreateExercise;

/// <summary>
/// Endpoint for creating a new exercise.
/// </summary>
/// <remarks>
/// Este endpoint aceita formulário multipart/form-data com dois campos:
/// - `metadata` — string contendo o JSON serializado de <see cref="CreateExerciseRequestDto"/>;
/// - `file` — arquivo opcional (ex.: enunciado em PDF).
/// Exemplo de uso via curl:
/// <code>
/// curl -X POST "https://localhost:5001/api/Exercise" \
///  -F "metadata={ \"title\": \"Somar dois números\", \"exerciseTypeId\": 1, \"inputs\": [{ \"orderId\": 1, \"input\": \"1 2\" }], \"outputs\": [{ \"orderId\": 1, \"output\": \"3\" }] }" \
///  -F "file=@statement.pdf"
/// </code>
/// </remarks>
public class CreateExerciseEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/Exercise", [Authorize(Roles = "Teacher,Admin")] async (
            IMediator mediator, 
            [FromForm] IFormFile? file,
            [FromForm] string metadata) =>
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var metadataDto = JsonSerializer.Deserialize<CreateExerciseRequestDto>(metadata, options);
            
            if (metadataDto == null)
            {
                return Results.BadRequest("Invalid metadata");
            }

            var command = new CreateExerciseCommand(metadataDto, file);
            var result = await mediator.Send(command);
            return Results.Ok(result);
        })
        .WithName("CreateExercise")
        .WithTags("Exercises")
        .DisableAntiforgery()
        .Produces<CreateExerciseResult>();
    }
} 
