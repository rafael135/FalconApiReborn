using Falcon.Api.Features.Admin.GenerateTeacherToken;
using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Falcon.Api.IntegrationTests.Features.Admin;

public class GenerateTeacherTokenTests : TestBase
{
    public GenerateTeacherTokenTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AdminGeneratesToken()
    {
        // Arrange
        var (_, token) = await CreateAdminAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.PostAsync("/api/Admin/teacher-token?expirationHours=168", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GenerateTeacherTokenResult>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.PostAsync("/api/Admin/teacher-token?expirationHours=168", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_StudentTriesToGenerateToken()
    {
        // Arrange
        var (_, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.PostAsync("/api/Admin/teacher-token?expirationHours=168", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_TeacherTriesToGenerateToken()
    {
        // Arrange
        var (_, token) = await CreateTeacherAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.PostAsync("/api/Admin/teacher-token?expirationHours=168", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
