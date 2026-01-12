using System.Text.Json;
using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

/// <summary>
/// Update an existing exercise. This endpoint accepts multipart/form-data.
/// </summary>
/// <remarks>
/// This endpoint expects a multipart form where:
/// - `metadata` is a string field containing JSON serialized <see cref="UpdateExerciseRequestDto"/>;
/// - `file` is an optional attached file.
/// Example curl usage:
/// <code>
/// curl -X PUT "https://localhost:5001/api/Exercise/00000000-0000-0000-0000-000000000000" \
///  -F "metadata={ \"id\": \"00000000-0000-0000-0000-000000000000\", \"title\": \"New Title\", \"exerciseTypeId\":1 }" \
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
        .WithSummary("Update an exercise.")
        .WithDescription("Updates an existing exercise using multipart/form-data; route id must match metadata.id.")
        .Accepts<IFormFile>("multipart/form-data")
        .Produces<UpdateExerciseResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status403Forbidden)
        .Produces(StatusCodes.Status404NotFound)
        .DisableAntiforgery();
    }
} 
