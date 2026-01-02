using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Users;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Groups;

public class GroupInviteTests
{
    [Fact]
    public void Constructor_Should_CreateInvite_WithValidParameters()
    {
        // Arrange
        var group = CreateTestGroup();
        var user = CreateTestUser("Invited User", "invited@test.com", "67890");

        // Act
        var invite = new GroupInvite(group, user);

        // Assert
        invite.Should().NotBeNull();
        invite.Group.Should().Be(group);
        invite.GroupId.Should().Be(group.Id);
        invite.User.Should().Be(user);
        invite.UserId.Should().Be(user.Id);
        invite.Accepted.Should().BeFalse();
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenGroupIsNull()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        Action act = () => new GroupInvite(null!, user);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("group");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenUserIsNull()
    {
        // Arrange
        var group = CreateTestGroup();

        // Act
        Action act = () => new GroupInvite(group, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("user");
    }

    [Fact]
    public void MarkAsAccepted_Should_SetAcceptedToTrue()
    {
        // Arrange
        var invite = CreateTestInvite();

        // Act
        invite.MarkAsAccepted();

        // Assert
        invite.Accepted.Should().BeTrue();
    }

    [Fact]
    public void MarkAsAccepted_Should_BeIdempotent()
    {
        // Arrange
        var invite = CreateTestInvite();

        // Act
        invite.MarkAsAccepted();
        invite.MarkAsAccepted();
        invite.MarkAsAccepted();

        // Assert
        invite.Accepted.Should().BeTrue();
    }

    [Fact]
    public void MarkAsAccepted_Should_MaintainGroupReference()
    {
        // Arrange
        var group = CreateTestGroup();
        var user = CreateTestUser();
        var invite = new GroupInvite(group, user);

        // Act
        invite.MarkAsAccepted();

        // Assert
        invite.Group.Should().Be(group);
        invite.GroupId.Should().Be(group.Id);
    }

    [Fact]
    public void MarkAsAccepted_Should_MaintainUserReference()
    {
        // Arrange
        var group = CreateTestGroup();
        var user = CreateTestUser();
        var invite = new GroupInvite(group, user);

        // Act
        invite.MarkAsAccepted();

        // Assert
        invite.User.Should().Be(user);
        invite.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void Invite_Should_SupportMultipleUsersForSameGroup()
    {
        // Arrange
        var group = CreateTestGroup();
        var user1 = CreateTestUser("User 1", "user1@test.com", "11111");
        var user2 = CreateTestUser("User 2", "user2@test.com", "22222");

        // Act
        var invite1 = new GroupInvite(group, user1);
        var invite2 = new GroupInvite(group, user2);

        // Assert
        invite1.GroupId.Should().Be(group.Id);
        invite2.GroupId.Should().Be(group.Id);
        invite1.UserId.Should().NotBe(invite2.UserId);
    }

    [Fact]
    public void Invite_Should_SupportPendingAndAcceptedInvites()
    {
        // Arrange
        var group = CreateTestGroup();
        var user1 = CreateTestUser("User 1", "user1@test.com", "11111");
        var user2 = CreateTestUser("User 2", "user2@test.com", "22222");

        // Act
        var pendingInvite = new GroupInvite(group, user1);
        var acceptedInvite = new GroupInvite(group, user2);
        acceptedInvite.MarkAsAccepted();

        // Assert
        pendingInvite.Accepted.Should().BeFalse();
        acceptedInvite.Accepted.Should().BeTrue();
    }

    [Fact]
    public void Invite_Should_PreserveInitialAcceptedStateFalse()
    {
        // Arrange
        var group = CreateTestGroup();
        var user = CreateTestUser();

        // Act
        var invite = new GroupInvite(group, user);

        // Assert
        invite.Accepted.Should().BeFalse();
    }

    [Fact]
    public void Constructor_Should_CreateInviteForEachCall()
    {
        // Arrange
        var group = CreateTestGroup();
        var user = CreateTestUser();

        // Act
        var invite1 = new GroupInvite(group, user);
        var invite2 = new GroupInvite(group, user);

        // Assert
        invite1.Should().NotBe(invite2);
        invite1.GroupId.Should().Be(group.Id);
        invite2.GroupId.Should().Be(group.Id);
    }

    // Helper methods
    private static GroupInvite CreateTestInvite()
    {
        return new GroupInvite(CreateTestGroup(), CreateTestUser());
    }

    private static Group CreateTestGroup()
    {
        var leader = new User("Leader", "leader@test.com", "12345");
        return new Group("Test Group", leader);
    }

    private static User CreateTestUser()
    {
        return CreateTestUser("Test User", "test@example.com", "54321");
    }

    private static User CreateTestUser(string name, string email, string ra)
    {
        return new User(name, email, ra);
    }
}
