using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Groups.Rules;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Users;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Groups;

public class GroupTests
{
    [Fact]
    public void Constructor_Should_CreateGroup_WithValidParameters()
    {
        // Arrange
        var groupName = "Test Group";
        var leader = CreateTestUser("leader@test.com", "Leader User");

        // Act
        var group = new Group(groupName, leader);

        // Assert
        group.Should().NotBeNull();
        group.Name.Should().Be(groupName);
        group.LeaderId.Should().Be(leader.Id);
        group.Users.Should().ContainSingle();
        group.Users.First().Id.Should().Be(leader.Id);
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var leader = CreateTestUser("leader@test.com", "Leader User");

        // Act
        Action act = () => new Group("", leader);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Nome do grupo é obrigatório");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenLeaderIsNull()
    {
        // Act
        Action act = () => new Group("Test Group", null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Rename_Should_UpdateGroupName_WithValidName()
    {
        // Arrange
        var group = CreateTestGroup("Old Name");
        var newName = "New Name";

        // Act
        group.Rename(newName);

        // Assert
        group.Name.Should().Be(newName);
    }

    [Fact]
    public void Rename_Should_ThrowArgumentException_WhenNameIsEmpty()
    {
        // Arrange
        var group = CreateTestGroup("Test Group");

        // Act
        Action act = () => group.Rename("");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Nome do grupo é obrigatório");
    }

    [Fact]
    public void AddMember_Should_AddUserToGroup_WhenUnderMaxMembers()
    {
        // Arrange
        var group = CreateTestGroup("Test Group");
        var newMember = CreateTestUser("member@test.com", "New Member");

        // Act
        group.AddMember(newMember);

        // Assert
        group.Users.Should().HaveCount(2); // Leader + new member
        group.Users.Should().Contain(u => u.Id == newMember.Id);
    }

    [Fact]
    public void AddMember_Should_ThrowBusinessRuleException_WhenExceedsMaxMembers()
    {
        // Arrange - Rule checks if _users.Count > 3 BEFORE adding
        // Group starts with 1 member (leader)
        var group = CreateTestGroup("Test Group");
        var member1 = CreateTestUser("member1@test.com", "Member 1");
        var member2 = CreateTestUser("member2@test.com", "Member 2");
        var member3 = CreateTestUser("member3@test.com", "Member 3");
        var member4 = CreateTestUser("member4@test.com", "Member 4"); 

        group.AddMember(member1);  // Check: 1 > 3? NO → Add → count = 2
        group.AddMember(member2);  // Check: 2 > 3? NO → Add → count = 3
        group.AddMember(member3);  // Check: 3 > 3? NO → Add → count = 4

        // Act - Now count is 4, trying to add 5th member
        // Check: 4 > 3? YES → Exception!
        Action act = () => group.AddMember(member4);

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*não pode ter mais de*membros*");
    }

    [Fact]
    public void RemoveMember_Should_RemoveUserFromGroup()
    {
        // Arrange
        var group = CreateTestGroup("Test Group");
        var member = CreateTestUser("member@test.com", "Member");
        group.AddMember(member);

        // Act
        group.RemoveMember(member);

        // Assert
        group.Users.Should().ContainSingle(); // Only leader remains
        group.Users.Should().NotContain(u => u.Id == member.Id);
    }

    [Fact]
    public void RemoveMember_Should_ThrowArgumentNullException_WhenUserIsNull()
    {
        // Arrange
        var group = CreateTestGroup("Test Group");

        // Act
        Action act = () => group.RemoveMember(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    // Helper methods
    private static Group CreateTestGroup(string name)
    {
        var leader = CreateTestUser("leader@test.com", "Leader User");
        return new Group(name, leader);
    }

    private static User CreateTestUser(string email, string name, int? joinYear = 2024)
    {
        return new User(name, email, "123456", joinYear, "Computer Science");
    }
}
