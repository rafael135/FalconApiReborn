using System.Text.Json;
using Falcon.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Falcon.Api.Features.Exercises.UpdateExercise;

public class UpdateExerciseEndpoint : IEndpoint
{
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
