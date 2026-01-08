using Falcon.Api.Features.Auth.RegisterUser;
using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Falcon.Api.IntegrationTests.Features.Auth;

public class RegisterUserTests : TestBase
{
    public RegisterUserTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_ValidStudentRegistration()
    {
        // Arrange
        var command = new
        {
            Ra = "123456",
            Name = "Test Student",
            Email = "student@test.com",
            Role = "Student",
            Password = "password123"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("user").Should().NotBeNull();
        jsonDoc.RootElement.GetProperty("token").GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_EmailAlreadyExists()
    {
        // Arrange
        var existingUser = await CreateStudentAsync("existing@test.com", "Existing User", "111111");
        
        var command = new
        {
            Ra = "999999",
            Name = "New User",
            Email = "existing@test.com",
            Role = "Student",
            Password = "password123"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_RAAlreadyExists()
    {
        // Arrange
        var existingUser = await CreateStudentAsync("user1@test.com", "User 1", "123456");
        
        var command = new
        {
            Ra = "123456",
            Name = "User 2",
            Email = "user2@test.com",
            Role = "Student",
            Password = "password123"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_TeacherRegistrationWithoutAccessCode()
    {
        // Arrange
        var command = new
        {
            Ra = "222222",
            Name = "Test Teacher",
            Email = "teacher@test.com",
            Role = "Teacher",
            Password = "password123"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_AdminRegistrationAttempt()
    {
        // Arrange
        var command = new
        {
            Ra = "333333",
            Name = "Test Admin",
            Email = "admin@test.com",
            Role = "Admin",
            Password = "password123"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Auth/register", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
