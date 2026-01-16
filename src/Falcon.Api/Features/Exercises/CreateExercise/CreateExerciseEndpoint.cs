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
/// This endpoint accepts multipart/form-data with two fields:
/// - `metadata` — a string containing JSON serialized <see cref="CreateExerciseRequestDto"/>;
/// - `file` — an optional file attachment (e.g., PDF statement).
/// Example curl usage:
/// <code>
/// curl -X POST "https://localhost:5001/api/Exercise" \
///  -F "metadata={ \"title\": \"Add numbers\", \"exerciseTypeId\": 1, \"inputs\": [{ \"orderId\": 1, \"input\": \"1 2\" }], \"outputs\": [{ \"orderId\": 1, \"output\": \"3\" }] }" \
///  -F "file=@statement.pdf"
/// </code>
/// </remarks>
public class CreateExerciseEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "api/Exercise",
                [Authorize(Roles = "Teacher,Admin")]
                async (IMediator mediator, [FromForm] CreateExerciseFormDto dto) =>
                {
                    var metadata = dto.Metadata;

                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                    var metadataDto = JsonSerializer.Deserialize<CreateExerciseRequestDto>(
                        metadata ?? string.Empty,
                        options
                    );

                    if (metadataDto == null)
                    {
                        return Results.BadRequest("Invalid metadata");
                    }

                    var command = new CreateExerciseCommand(metadataDto, dto.File);
                    var result = await mediator.Send(command);
                    return Results.Ok(result);
                }
            )
            .WithName("CreateExercise")
            .WithTags("Exercises")
            .WithSummary("Create a new exercise.")
            .WithDescription(
                "Creates a new exercise using multipart/form-data: 'metadata' JSON string and optional 'file' attachment."
            )
            .DisableAntiforgery()
            .WithMetadata(new Microsoft.AspNetCore.Mvc.ConsumesAttribute("multipart/form-data"))
            .Produces<CreateExerciseResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}
