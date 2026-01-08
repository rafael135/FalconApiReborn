using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Falcon.Api.IntegrationTests.Features.Groups;

public class InviteUserTests : TestBase
{
    public InviteUserTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_ValidInvite()
    {
        // Arrange
        var (leader, leaderToken) = await CreateStudentAsync("leader@test.com", "Leader", "111111");
        var (invitedUser, _) = await CreateStudentAsync("invited@test.com", "Invited", "222222");
        var group = await CreateGroupAsync(leader, "Test Group");
        HttpClient.SetBearerToken(leaderToken);

        var command = new
        {
            GroupId = group.Id,
            RA = "222222" // Parameter name should match InviteUserCommand
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Group/invite", command);

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
            GroupId = Guid.NewGuid(),
            RA = "123456"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Group/invite", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
