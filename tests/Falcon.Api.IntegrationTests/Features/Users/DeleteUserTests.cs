using System.Net;
using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Api.IntegrationTests.Features.Users;

public class DeleteUserTests : TestBase
{
    public DeleteUserTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_AdminDeletesUser()
    {
        // Arrange
        var (student, _) = await CreateStudentAsync();
        var (admin, adminToken) = await CreateAdminAsync();
        HttpClient.SetBearerToken(adminToken);

        // Act
        var response = await HttpClient.DeleteAsync($"/api/User/{student.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Clear tracked entities to force reload from database
        DbContext.ChangeTracker.Clear();

        // Verify user is soft deleted
        var deletedUser = await DbContext.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == student.Id);
        deletedUser.Should().NotBeNull();
        deletedUser!.IsDeleted.Should().BeTrue();
        deletedUser.DeletedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_UserDeletesOwnAccount()
    {
        // Arrange
        var (user, token) = await CreateStudentAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.DeleteAsync($"/api/User/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnForbidden_When_StudentTriesToDeleteOtherUser()
    {
        // Arrange
        var (student1, token1) = await CreateStudentAsync("student1@test.com", "Student 1", "111111");
        var (student2, _) = await CreateStudentAsync("student2@test.com", "Student 2", "222222");
        HttpClient.SetBearerToken(token1);

        // Act
        var response = await HttpClient.DeleteAsync($"/api/User/{student2.Id}");

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
        var response = await HttpClient.DeleteAsync($"/api/User/{user.Id}");

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
        var response = await HttpClient.DeleteAsync($"/api/User/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_TransferLeadership_When_DeletingLeaderWithMembers()
    {
        // Arrange
        var (leader, leaderToken) = await CreateStudentAsync("leader@test.com", "Leader", "111111");
        var (member1, _) = await CreateStudentAsync("member1@test.com", "Alpha Member", "222222");
        var (member2, _) = await CreateStudentAsync("member2@test.com", "Beta Member", "333333");

        var group = await CreateGroupAsync(leader, "Test Group");

        // Manually add members to group
        group.AddMember(member1);
        member1.AssignGroup(group);
        group.AddMember(member2);
        member2.AssignGroup(group);
        await DbContext.SaveChangesAsync();

        var (admin, adminToken) = await CreateAdminAsync();
        HttpClient.SetBearerToken(adminToken);

        // Act
        var response = await HttpClient.DeleteAsync($"/api/User/{leader.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Clear tracked entities to force reload from database with query filter applied
        DbContext.ChangeTracker.Clear();

        // Verify leadership transferred to first member alphabetically (Alpha Member)
        var updatedGroup = await DbContext.Groups
            .Include(g => g.Users)
            .FirstOrDefaultAsync(g => g.Id == group.Id);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.LeaderId.Should().Be(member1.Id); // Alpha comes before Beta
    }

    [Fact]
    public async Task Should_DisbandGroup_When_DeletingSoloLeader()
    {
        // Arrange
        var (leader, _) = await CreateStudentAsync();
        var group = await CreateGroupAsync(leader, "Solo Group");

        var (admin, adminToken) = await CreateAdminAsync();
        HttpClient.SetBearerToken(adminToken);

        // Act
        var response = await HttpClient.DeleteAsync($"/api/User/{leader.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Clear tracked entities to force reload from database with query filter applied
        DbContext.ChangeTracker.Clear();

        // Verify group has no members
        var updatedGroup = await DbContext.Groups
            .Include(g => g.Users)
            .FirstOrDefaultAsync(g => g.Id == group.Id);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Users.Count.Should().Be(0);
    }

    [Fact]
    public async Task Should_RemoveFromGroup_When_DeletingNonLeaderMember()
    {
        // Arrange
        var (leader, _) = await CreateStudentAsync("leader@test.com", "Leader", "111111");
        var (member, _) = await CreateStudentAsync("member@test.com", "Member", "222222");

        var group = await CreateGroupAsync(leader, "Test Group");
        group.AddMember(member);
        member.AssignGroup(group);
        await DbContext.SaveChangesAsync();

        var (admin, adminToken) = await CreateAdminAsync();
        HttpClient.SetBearerToken(adminToken);

        // Act
        var response = await HttpClient.DeleteAsync($"/api/User/{member.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Clear tracked entities to force reload from database with query filter applied
        DbContext.ChangeTracker.Clear();

        // Verify member removed from group
        var updatedGroup = await DbContext.Groups
            .Include(g => g.Users)
            .FirstOrDefaultAsync(g => g.Id == group.Id);
        updatedGroup.Should().NotBeNull();
        updatedGroup!.Users.Should().NotContain(u => u.Id == member.Id);
        updatedGroup.LeaderId.Should().Be(leader.Id); // Leadership unchanged
    }

    [Fact]
    public async Task Should_CreateAuditLog_When_DeletingUser()
    {
        // Arrange
        var (student, _) = await CreateStudentAsync();
        var (admin, adminToken) = await CreateAdminAsync();
        HttpClient.SetBearerToken(adminToken);

        var logCountBefore = await DbContext.Logs.CountAsync();

        // Act
        var response = await HttpClient.DeleteAsync($"/api/User/{student.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var logCountAfter = await DbContext.Logs.CountAsync();
        logCountAfter.Should().BeGreaterThan(logCountBefore);
    }
}
