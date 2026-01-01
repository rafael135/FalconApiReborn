using System.Text.Json;
using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.CreateExercise;

/// <summary>
/// Endpoint for creating a new exercise.
/// </summary>
public class CreateExerciseEndpoint : IEndpoint
{
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
