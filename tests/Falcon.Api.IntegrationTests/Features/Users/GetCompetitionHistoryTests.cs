using System.Net;
using System.Text.Json;
using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;

namespace Falcon.Api.IntegrationTests.Features.Users;

public class GetCompetitionHistoryTests : TestBase
{
    public GetCompetitionHistoryTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_UserGetsOwnHistory()
    {
        // Arrange
        var (user, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{user.Id}/competition-history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("items").ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AdminGetsAnyUserHistory()
    {
        // Arrange
        var (student, _) = await CreateStudentAsync();
        var (admin, adminToken) = await CreateAdminAsync();
        HttpClient.SetBearerToken(adminToken);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{student.Id}/competition-history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_StudentTriesToGetOtherUserHistory()
    {
        // Arrange
        var (student1, token1) = await CreateStudentAsync("student1@test.com", "Student 1", "111111");
        var (student2, _) = await CreateStudentAsync("student2@test.com", "Student 2", "222222");
        HttpClient.SetBearerToken(token1);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{student2.Id}/competition-history");

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
        var response = await HttpClient.GetAsync($"/api/User/{user.Id}/competition-history");

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
        var response = await HttpClient.GetAsync($"/api/User/{nonExistentId}/competition-history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_ReturnEmptyArray_When_UserHasNoCompetitionHistory()
    {
        // Arrange
        var (user, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{user.Id}/competition-history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("items").GetArrayLength().Should().Be(0);
        jsonDoc.RootElement.GetProperty("total").GetInt32().Should().Be(0);
    }

    [Fact]
    public async Task Should_ReturnPaginatedResults_When_SkipAndTakeProvided()
    {
        // Arrange
        var (user, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{user.Id}/competition-history?skip=0&take=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("skip").GetInt32().Should().Be(0);
        jsonDoc.RootElement.GetProperty("take").GetInt32().Should().Be(5);
    }

    [Fact]
    public async Task Should_IncludePaginationInfo_When_ReturningHistory()
    {
        // Arrange
        var (user, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/User/{user.Id}/competition-history");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.TryGetProperty("total", out _).Should().BeTrue();
        jsonDoc.RootElement.TryGetProperty("skip", out _).Should().BeTrue();
        jsonDoc.RootElement.TryGetProperty("take", out _).Should().BeTrue();
        jsonDoc.RootElement.TryGetProperty("items", out _).Should().BeTrue();
    }
}
