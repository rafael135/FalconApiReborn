using Falcon.Api.IntegrationTests.Helpers;
using Falcon.Core.Domain.Groups;
using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Groups;

public class RejectInviteTests : TestBase
{
    public RejectInviteTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_RejectingInvite()
    {
        // Arrange
        var (leader, _) = await CreateStudentAsync("leader@test.com", "Leader", "111111");
        var (invitedUser, invitedToken) = await CreateStudentAsync("invited@test.com", "Invited", "222222");
        var group = await CreateGroupAsync(leader, "Test Group");
        
        var invite = new GroupInvite(group, invitedUser);
        DbContext.GroupInvites.Add(invite);
        await DbContext.SaveChangesAsync();

        HttpClient.SetBearerToken(invitedToken);

        // Act
        var response = await HttpClient.PostAsync($"/api/Group/invite/{invite.Id}/reject", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.PostAsync($"/api/Group/invite/{Guid.NewGuid()}/reject", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
