using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Groups;

public class RemoveMemberTests : TestBase
{
    public RemoveMemberTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_RemovingMember()
    {
        // Arrange
        var (leader, leaderToken) = await CreateStudentAsync("leader@test.com", "Leader", "111111");
        var (member, _) = await CreateStudentAsync("member@test.com", "Member", "222222");
        var group = await CreateGroupAsync(leader, "Test Group");
        group.AddMember(member);
        member.AssignGroup(group);
        await DbContext.SaveChangesAsync();

        HttpClient.SetBearerToken(leaderToken);

        // Act
        var response = await HttpClient.DeleteAsync($"/api/Group/{group.Id}/member/{member.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.DeleteAsync($"/api/Group/{Guid.NewGuid()}/member/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
