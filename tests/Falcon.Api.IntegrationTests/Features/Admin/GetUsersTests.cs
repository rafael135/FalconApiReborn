using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Admin;

public class GetUsersTests : TestBase
{
    public GetUsersTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AdminGetsUsers()
    {
        // Arrange
        var (admin, token) = await CreateAdminAsync();
        await CreateStudentAsync();
        await CreateTeacherAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_FilteringByRole()
    {
        // Arrange
        var (admin, token) = await CreateAdminAsync();
        await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/users?role=Student");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_StudentTriesToGetUsers()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync("/api/Admin/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
