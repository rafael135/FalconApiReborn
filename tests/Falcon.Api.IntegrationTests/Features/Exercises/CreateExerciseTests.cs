using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Falcon.Api.IntegrationTests.Features.Exercises;

public class CreateExerciseTests : TestBase
{
    public CreateExerciseTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_TeacherCreatesExercise()
    {
        // Arrange
        var (teacher, token) = await CreateTeacherAsync();
        var exerciseType = await GetOrCreateExerciseTypeAsync();
        HttpClient.SetBearerToken(token);

        var metadata = new
        {
            Title = "Test Exercise",
            Description = "Test Description",
            ExerciseTypeId = exerciseType.Id,
            EstimatedTime = TimeSpan.FromMinutes(30).ToString(@"hh\:mm\:ss"),
            Inputs = new List<object>(),
            Outputs = new List<object>()
        };

        using var content = new MultipartFormDataContent();
        var metadataJson = JsonSerializer.Serialize(metadata);
        content.Add(new StringContent(metadataJson, Encoding.UTF8, "application/json"), "metadata");

        // Act
        var response = await HttpClient.PostAsync("/api/Exercise", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        var metadata = new
        {
            Title = "Test Exercise",
            Description = "Test Description",
            ExerciseTypeId = 1,
            EstimatedTime = TimeSpan.FromMinutes(30).ToString(@"hh\:mm\:ss"),
            Inputs = new List<object>(),
            Outputs = new List<object>()
        };

        using var content = new MultipartFormDataContent();
        var metadataJson = JsonSerializer.Serialize(metadata);
        content.Add(new StringContent(metadataJson, Encoding.UTF8, "application/json"), "metadata");

        // Act
        var response = await HttpClient.PostAsync("/api/Exercise", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
