using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Admin;

public class GetStatisticsTests : TestBase
{
    public GetStatisticsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AdminGetsStatistics()
    {
        // Arrange
        var (admin, token) = await CreateAdminAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_StudentTriesToGetStatistics()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/stats");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
