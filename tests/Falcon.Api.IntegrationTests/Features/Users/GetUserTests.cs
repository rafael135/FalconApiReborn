using System.Net;
using System.Text.Json;
using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;

namespace Falcon.Api.IntegrationTests.Features.Users;

public class GetUserTests : TestBase
{
    public GetUserTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_UserGetsOwnProfile()
    {
        // Arrange
        var (user, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("id").GetString().Should().Be(user.Id);
        jsonDoc.RootElement.GetProperty("email").GetString().Should().Be(user.Email);
        jsonDoc.RootElement.GetProperty("name").GetString().Should().Be(user.Name);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AdminGetsAnyUserProfile()
    {
        // Arrange
        var (student, _) = await CreateStudentAsync();
        var (admin, adminToken) = await CreateAdminAsync();
        HttpClient.SetBearerToken(adminToken);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{student.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("id").GetString().Should().Be(student.Id);
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_StudentTriesToGetOtherUserProfile()
    {
        // Arrange
        var (student1, token1) = await CreateStudentAsync("student1@test.com", "Student 1", "111111");
        var (student2, _) = await CreateStudentAsync("student2@test.com", "Student 2", "222222");
        HttpClient.SetBearerToken(token1);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{student2.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        var (user, _) = await CreateStudentAsync();
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{user.Id}");

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

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_IncludeGroupInformation_When_UserHasGroup()
    {
        // Arrange
        var (leader, token) = await CreateStudentAsync();
        var group = await CreateGroupAsync(leader, "Test Group");
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{leader.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("groupId").GetGuid().Should().Be(group.Id);
        jsonDoc.RootElement.GetProperty("groupName").GetString().Should().Be(group.Name);
    }

    [Fact]
    public async Task Should_IncludeRoles_When_GettingUserProfile()
    {
        // Arrange
        var (teacher, token) = await CreateTeacherAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{teacher.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        var roles = jsonDoc.RootElement.GetProperty("roles");
        roles.GetArrayLength().Should().BeGreaterThan(0);
    }
}
