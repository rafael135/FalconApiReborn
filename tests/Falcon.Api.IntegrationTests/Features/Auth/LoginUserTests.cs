using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Falcon.Api.IntegrationTests.Features.Auth;

public class LoginUserTests : TestBase
{
    public LoginUserTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_ValidCredentials()
    {
        // Arrange
        var (user, _) = await CreateStudentAsync("login@test.com", "Login User", "444444", "password123");
        
        var command = new
        {
            RA = "444444",
            Password = "password123"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("user").Should().NotBeNull();
        jsonDoc.RootElement.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_InvalidRA()
    {
        // Arrange
        var command = new
        {
            RA = "999999",
            Password = "password123"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_InvalidPassword()
    {
        // Arrange
        var (user, _) = await CreateStudentAsync("wrongpass@test.com", "Wrong Pass User", "555555", "password123");
        
        var command = new
        {
            RA = "555555",
            Password = "wrongpassword"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Auth/login", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
