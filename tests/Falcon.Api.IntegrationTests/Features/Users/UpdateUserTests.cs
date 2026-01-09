using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;

namespace Falcon.Api.IntegrationTests.Features.Users;

public class UpdateUserTests : TestBase
{
    public UpdateUserTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_UserUpdatesOwnProfile()
    {
        // Arrange
        var (user, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        var command = new
        {
            UserId = user.Id,
            Name = "Updated Name",
            Email = user.Email,
            RA = user.RA,
            JoinYear = 2024,
            Department = "Computer Science",
            CurrentPassword = (string?)null,
            NewPassword = (string?)null
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{user.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("name").GetString().Should().Be("Updated Name");
        jsonDoc.RootElement.GetProperty("department").GetString().Should().Be("Computer Science");
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AdminUpdatesAnyUserProfile()
    {
        // Arrange
        var (student, _) = await CreateStudentAsync();
        var (admin, adminToken) = await CreateAdminAsync();
        HttpClient.SetBearerToken(adminToken);

        var command = new
        {
            UserId = student.Id,
            Name = "Admin Updated Name",
            Email = student.Email,
            RA = student.RA,
            JoinYear = (int?)null,
            Department = "Engineering",
            CurrentPassword = (string?)null,
            NewPassword = (string?)null
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{student.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("name").GetString().Should().Be("Admin Updated Name");
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_StudentTriesToUpdateOtherUserProfile()
    {
        // Arrange
        var (student1, token1) = await CreateStudentAsync("student1@test.com", "Student 1", "111111");
        var (student2, _) = await CreateStudentAsync("student2@test.com", "Student 2", "222222");
        HttpClient.SetBearerToken(token1);

        var command = new
        {
            UserId = student2.Id,
            Name = "Hacked Name",
            Email = student2.Email,
            RA = student2.RA,
            JoinYear = (int?)null,
            Department = (string?)null,
            CurrentPassword = (string?)null,
            NewPassword = (string?)null
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{student2.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_EmailAlreadyExists()
    {
        // Arrange
        var (user1, token1) = await CreateStudentAsync("user1@test.com", "User 1", "111111");
        var (user2, _) = await CreateStudentAsync("user2@test.com", "User 2", "222222");
        HttpClient.SetBearerToken(token1);

        var command = new
        {
            UserId = user1.Id,
            Name = user1.Name,
            Email = "user2@test.com", // Trying to use existing email
            RA = user1.RA,
            JoinYear = (int?)null,
            Department = (string?)null,
            CurrentPassword = (string?)null,
            NewPassword = (string?)null
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{user1.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_RAAlreadyExists()
    {
        // Arrange
        var (user1, token1) = await CreateStudentAsync("user1@test.com", "User 1", "111111");
        var (user2, _) = await CreateStudentAsync("user2@test.com", "User 2", "222222");
        HttpClient.SetBearerToken(token1);

        var command = new
        {
            UserId = user1.Id,
            Name = user1.Name,
            Email = user1.Email,
            RA = "222222", // Trying to use existing RA
            JoinYear = (int?)null,
            Department = (string?)null,
            CurrentPassword = (string?)null,
            NewPassword = (string?)null
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{user1.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        var (user, _) = await CreateStudentAsync();
        HttpClient.ClearAuthorization();

        var command = new
        {
            UserId = user.Id,
            Name = "New Name",
            Email = user.Email,
            RA = user.RA,
            JoinYear = (int?)null,
            Department = (string?)null,
            CurrentPassword = (string?)null,
            NewPassword = (string?)null
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{user.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_UserDoesNotExist()
    {
        // Arrange
        var (admin, token) = await CreateAdminAsync();
        var nonExistentId = Guid.NewGuid().ToString();
        HttpClient.SetBearerToken(token);

        var command = new
        {
            UserId = nonExistentId,
            Name = "New Name",
            Email = "test@test.com",
            RA = "999999",
            JoinYear = (int?)null,
            Department = (string?)null,
            CurrentPassword = (string?)null,
            NewPassword = (string?)null
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{nonExistentId}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_RouteIdDoesNotMatchCommandId()
    {
        // Arrange
        var (user, token) = await CreateStudentAsync();
        var otherId = Guid.NewGuid().ToString();
        HttpClient.SetBearerToken(token);

        var command = new
        {
            UserId = user.Id,
            Name = "New Name",
            Email = user.Email,
            RA = user.RA,
            JoinYear = (int?)null,
            Department = (string?)null,
            CurrentPassword = (string?)null,
            NewPassword = (string?)null
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{otherId}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_UpdatePassword_When_CurrentPasswordIsCorrect()
    {
        // Arrange
        var password = "password123";
        var (user, token) = await CreateStudentAsync(password: password);
        HttpClient.SetBearerToken(token);

        var command = new
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            RA = user.RA,
            JoinYear = (int?)null,
            Department = (string?)null,
            CurrentPassword = password,
            NewPassword = "newPassword456"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{user.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnBadRequest_When_CurrentPasswordIsIncorrect()
    {
        // Arrange
        var (user, token) = await CreateStudentAsync(password: "password123");
        HttpClient.SetBearerToken(token);

        var command = new
        {
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email,
            RA = user.RA,
            JoinYear = (int?)null,
            Department = (string?)null,
            CurrentPassword = "wrongPassword",
            NewPassword = "newPassword456"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/User/{user.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
