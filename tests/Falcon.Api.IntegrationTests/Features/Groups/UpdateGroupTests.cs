using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Falcon.Api.IntegrationTests.Features.Groups;

public class UpdateGroupTests : TestBase
{
    public UpdateGroupTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_ValidUpdate()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var group = await CreateGroupAsync(student, "Old Name");
        HttpClient.SetBearerToken(token);

        var command = new
        {
            GroupId = group.Id,
            Name = "New Name"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/Group/{group.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        var (student, _) = await CreateStudentAsync();
        var group = await CreateGroupAsync(student, "Test Group");
        HttpClient.ClearAuthorization();

        var command = new
        {
            GroupId = group.Id,
            Name = "New Name"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/Group/{group.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
