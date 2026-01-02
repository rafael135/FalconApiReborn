using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Users;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Competitions;

public class GroupInCompetitionTests
{
    [Fact]
    public void Constructor_Should_CreateGroupInCompetition_WithValidParameters()
    {
        // Arrange
        var group = CreateTestGroup();
        var competition = CreateTestCompetition();

        // Act
        var groupInCompetition = new GroupInCompetition(group, competition);

        // Assert
        groupInCompetition.Should().NotBeNull();
        groupInCompetition.Group.Should().Be(group);
        groupInCompetition.GroupId.Should().Be(group.Id);
        groupInCompetition.Competition.Should().Be(competition);
        groupInCompetition.CompetitionId.Should().Be(competition.Id);
        groupInCompetition.Blocked.Should().BeFalse();
        groupInCompetition.CreatedOn.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenGroupIsNull()
    {
        // Arrange
        var competition = CreateTestCompetition();

        // Act
        Action act = () => new GroupInCompetition(null!, competition);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("group");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenCompetitionIsNull()
    {
        // Arrange
        var group = CreateTestGroup();

        // Act
        Action act = () => new GroupInCompetition(group, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("competition");
    }

    [Fact]
    public void Block_Should_SetBlockedToTrue()
    {
        // Arrange
        var groupInCompetition = CreateTestGroupInCompetition();

        // Act
        groupInCompetition.Block();

        // Assert
        groupInCompetition.Blocked.Should().BeTrue();
    }

    [Fact]
    public void Unblock_Should_SetBlockedToFalse()
    {
        // Arrange
        var groupInCompetition = CreateTestGroupInCompetition();
        groupInCompetition.Block();

        // Act
        groupInCompetition.Unblock();

        // Assert
        groupInCompetition.Blocked.Should().BeFalse();
    }

    [Fact]
    public void Block_Should_BeIdempotent()
    {
        // Arrange
        var groupInCompetition = CreateTestGroupInCompetition();

        // Act
        groupInCompetition.Block();
        groupInCompetition.Block();
        groupInCompetition.Block();

        // Assert
        groupInCompetition.Blocked.Should().BeTrue();
    }

    [Fact]
    public void Unblock_Should_BeIdempotent()
    {
        // Arrange
        var groupInCompetition = CreateTestGroupInCompetition();

        // Act
        groupInCompetition.Unblock();
        groupInCompetition.Unblock();
        groupInCompetition.Unblock();

        // Assert
        groupInCompetition.Blocked.Should().BeFalse();
    }

    [Fact]
    public void Should_ToggleBetweenBlockedAndUnblockedStates()
    {
        // Arrange
        var groupInCompetition = CreateTestGroupInCompetition();

        // Act & Assert
        groupInCompetition.Blocked.Should().BeFalse();

        groupInCompetition.Block();
        groupInCompetition.Blocked.Should().BeTrue();

        groupInCompetition.Unblock();
        groupInCompetition.Blocked.Should().BeFalse();

        groupInCompetition.Block();
        groupInCompetition.Blocked.Should().BeTrue();
    }

    [Fact]
    public void Block_Should_MaintainGroupAndCompetitionReferences()
    {
        // Arrange
        var group = CreateTestGroup();
        var competition = CreateTestCompetition();
        var groupInCompetition = new GroupInCompetition(group, competition);

        // Act
        groupInCompetition.Block();

        // Assert
        groupInCompetition.Group.Should().Be(group);
        groupInCompetition.GroupId.Should().Be(group.Id);
        groupInCompetition.Competition.Should().Be(competition);
        groupInCompetition.CompetitionId.Should().Be(competition.Id);
    }

    [Fact]
    public void Unblock_Should_MaintainGroupAndCompetitionReferences()
    {
        // Arrange
        var group = CreateTestGroup();
        var competition = CreateTestCompetition();
        var groupInCompetition = new GroupInCompetition(group, competition);
        groupInCompetition.Block();

        // Act
        groupInCompetition.Unblock();

        // Assert
        groupInCompetition.Group.Should().Be(group);
        groupInCompetition.GroupId.Should().Be(group.Id);
        groupInCompetition.Competition.Should().Be(competition);
        groupInCompetition.CompetitionId.Should().Be(competition.Id);
    }

    [Fact]
    public void CreatedOn_Should_NotChangeAfterBlocking()
    {
        // Arrange
        var groupInCompetition = CreateTestGroupInCompetition();
        var originalCreatedOn = groupInCompetition.CreatedOn;

        // Act
        Thread.Sleep(10);
        groupInCompetition.Block();

        // Assert
        groupInCompetition.CreatedOn.Should().Be(originalCreatedOn);
    }

    [Fact]
    public void CreatedOn_Should_NotChangeAfterUnblocking()
    {
        // Arrange
        var groupInCompetition = CreateTestGroupInCompetition();
        groupInCompetition.Block();
        var originalCreatedOn = groupInCompetition.CreatedOn;

        // Act
        Thread.Sleep(10);
        groupInCompetition.Unblock();

        // Assert
        groupInCompetition.CreatedOn.Should().Be(originalCreatedOn);
    }

    // Helper methods
    private static GroupInCompetition CreateTestGroupInCompetition()
    {
        return new GroupInCompetition(CreateTestGroup(), CreateTestCompetition());
    }

    private static Group CreateTestGroup()
    {
        var leader = new User("Leader", "leader@test.com", "12345");
        return new Group("Test Group", leader);
    }

    private static Competition CreateTestCompetition()
    {
        return Competition.CreateTemplate(
            "Test Competition",
            "Description",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(3));
    }
}
