using Microsoft.AspNetCore.Http;

namespace Falcon.Api.Features.Exercises.CreateExercise
{
    /// <summary>
    /// Helper DTO used to bind multipart/form-data for the CreateExercise endpoint.
    /// </summary>
    public class CreateExerciseFormDto
    {
        /// <summary>
        /// Optional attached file (e.g. statement PDF).
        /// </summary>
        public IFormFile? File { get; set; }

        /// <summary>
        /// JSON string containing the <see cref="CreateExerciseRequestDto"/> metadata.
        /// </summary>
        public string? Metadata { get; set; }
    }
}
