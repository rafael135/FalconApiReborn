using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Exercises;

public class GetExercisesTests : TestBase
{
    public GetExercisesTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_GettingExercises()
    {
        // Arrange
        await CreateExerciseAsync();

        // Act
        var response = await HttpClient.GetAsync("/api/Exercise");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
