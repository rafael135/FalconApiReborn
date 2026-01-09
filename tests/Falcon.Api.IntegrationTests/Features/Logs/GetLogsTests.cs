using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Logs;

public class GetLogsTests : TestBase
{
    public GetLogsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AdminGetsLogs()
    {
        // Arrange
        var (admin, token) = await CreateAdminAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync("/api/Log");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.GetAsync("/api/Log");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
