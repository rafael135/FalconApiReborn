using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Falcon.Api.IntegrationTests.Features.Groups;

public class CreateGroupTests : TestBase
{
    public CreateGroupTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_ValidGroupCreation()
    {
        // Arrange
        var (_, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        var command = new
        {
            Name = "Test Group"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Group", command);

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
            Name = "Test Group"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Group", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
