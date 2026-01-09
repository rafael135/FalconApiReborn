using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Falcon.Api.IntegrationTests.Features.Competitions;

public class CreateTemplateTests : TestBase
{
    public CreateTemplateTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_TeacherCreatesTemplate()
    {
        // Arrange
        var (teacher, token) = await CreateTeacherAsync();
        HttpClient.SetBearerToken(token);

        var command = new
        {
            Name = "Test Competition",
            Description = "Test Description",
            StartInscriptions = DateTime.UtcNow.AddDays(-1),
            EndInscriptions = DateTime.UtcNow.AddDays(1),
            StartTime = DateTime.UtcNow.AddDays(2)
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Competition/template", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        var command = new
        {
            Name = "Test Competition",
            Description = "Test Description",
            StartInscriptions = DateTime.UtcNow.AddDays(-1),
            EndInscriptions = DateTime.UtcNow.AddDays(1),
            StartTime = DateTime.UtcNow.AddDays(2)
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Competition/template", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_StudentTriesToCreateTemplate()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        var command = new
        {
            Name = "Test Competition",
            Description = "Test Description",
            StartInscriptions = DateTime.UtcNow.AddDays(-1),
            EndInscriptions = DateTime.UtcNow.AddDays(1),
            StartTime = DateTime.UtcNow.AddDays(2)
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Competition/template", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
