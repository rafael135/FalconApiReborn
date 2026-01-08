using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;

namespace Falcon.Api.IntegrationTests.Features.Groups;

public class LeaveGroupTests : TestBase
{
    public LeaveGroupTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_UserLeavesGroup()
    {
        // Arrange
        // Create leader
        var (leader, _) = await CreateStudentAsync("leader@test.com", "Leader", "111111");
        var group = await CreateGroupAsync(leader, "Test Group");
        
        // Create member who will leave
        var (student, token) = await CreateStudentAsync("student@test.com", "Student", "222222");
        
        // Add student as member (not leader)
        group.AddMember(student);
        student.AssignGroup(group);
        await DbContext.SaveChangesAsync();
        
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.PostAsync("/api/Group/leave", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.PostAsync("/api/Group/leave", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
