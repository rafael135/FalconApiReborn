using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Groups;

public class GetGroupTests : TestBase
{
    public GetGroupTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_GroupExists()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var group = await CreateGroupAsync(student, "Test Group");
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/Group/{group.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_GroupDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await HttpClient.GetAsync($"/api/Group/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
