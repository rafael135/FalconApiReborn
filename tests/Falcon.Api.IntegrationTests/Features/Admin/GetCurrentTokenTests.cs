using Falcon.Api.Features.Admin.GetCurrentToken;
using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Falcon.Api.IntegrationTests.Features.Admin;

public class GetCurrentTokenTests : TestBase
{
    public GetCurrentTokenTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AdminGetsCurrentToken()
    {
        // Arrange
        var (admin, token) = await CreateAdminAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/teacher-token");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<GetCurrentTokenResult>();
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/teacher-token");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_StudentTriesToGetToken()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/teacher-token");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
