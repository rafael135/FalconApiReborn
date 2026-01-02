using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Users;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Auditing;

public class LogTests
{
    [Fact]
    public void Constructor_Should_CreateLog_WithAllParameters()
    {
        // Arrange
        var actionType = LogType.Login;
        var ipAddress = "192.168.1.1";
        var user = CreateTestUser();
        var group = CreateTestGroup();
        var competition = CreateTestCompetition();

        // Act
        var log = new Log(actionType, ipAddress, user, group, competition);

        // Assert
        log.Should().NotBeNull();
        log.ActionType.Should().Be(actionType);
        log.IpAddress.Should().Be(ipAddress);
        log.User.Should().Be(user);
        log.UserId.Should().Be(user.Id);
        log.Group.Should().Be(group);
        log.GroupId.Should().Be(group.Id);
        log.Competition.Should().Be(competition);
        log.CompetitionId.Should().Be(competition.Id);
        log.ActionTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_CreateLog_WithMinimalParameters()
    {
        // Arrange
        var actionType = LogType.Login;
        var ipAddress = "192.168.1.1";

        // Act
        var log = new Log(actionType, ipAddress);

        // Assert
        log.Should().NotBeNull();
        log.ActionType.Should().Be(actionType);
        log.IpAddress.Should().Be(ipAddress);
        log.User.Should().BeNull();
        log.UserId.Should().BeNull();
        log.Group.Should().BeNull();
        log.GroupId.Should().BeNull();
        log.Competition.Should().BeNull();
        log.CompetitionId.Should().BeNull();
        log.ActionTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_CreateLog_WithOnlyUser()
    {
        // Arrange
        var actionType = LogType.Login;
        var ipAddress = "192.168.1.1";
        var user = CreateTestUser();

        // Act
        var log = new Log(actionType, ipAddress, user);

        // Assert
        log.User.Should().Be(user);
        log.UserId.Should().Be(user.Id);
        log.Group.Should().BeNull();
        log.Competition.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenIpAddressIsInvalid(string? invalidIp)
    {
        // Act
        Action act = () => new Log(LogType.Login, invalidIp!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Endereço IP*obrigatório*");
    }

    [Theory]
    [InlineData(LogType.Login)]
    [InlineData(LogType.Logout)]
    [InlineData(LogType.CreateGroup)]
    [InlineData(LogType.JoinGroup)]
    [InlineData(LogType.LeaveGroup)]
    [InlineData(LogType.JoinCompetition)]
    [InlineData(LogType.LeaveCompetition)]
    [InlineData(LogType.SubmitExercise)]
    [InlineData(LogType.ViewRanking)]
    [InlineData(LogType.ViewExercise)]
    [InlineData(LogType.DownloadFile)]
    [InlineData(LogType.QuestionSent)]
    [InlineData(LogType.AnswerGiven)]
    public void Constructor_Should_AcceptAllLogTypes(LogType logType)
    {
        // Arrange
        var ipAddress = "192.168.1.1";

        // Act
        var log = new Log(logType, ipAddress);

        // Assert
        log.ActionType.Should().Be(logType);
    }

    [Fact]
    public void Constructor_Should_AcceptIPv4Address()
    {
        // Arrange
        var ipAddress = "192.168.1.100";

        // Act
        var log = new Log(LogType.Login, ipAddress);

        // Assert
        log.IpAddress.Should().Be(ipAddress);
    }

    [Fact]
    public void Constructor_Should_AcceptIPv6Address()
    {
        // Arrange
        var ipAddress = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";

        // Act
        var log = new Log(LogType.Login, ipAddress);

        // Assert
        log.IpAddress.Should().Be(ipAddress);
    }

    [Fact]
    public void Constructor_Should_AcceptLocalhostAddress()
    {
        // Arrange
        var ipAddress = "127.0.0.1";

        // Act
        var log = new Log(LogType.Login, ipAddress);

        // Assert
        log.IpAddress.Should().Be(ipAddress);
    }

    [Fact]
    public void Constructor_Should_SetActionTimeToUtcNow()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var log = new Log(LogType.Login, "192.168.1.1");

        // Assert
        log.ActionTime.Should().BeOnOrAfter(beforeCreation);
        log.ActionTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Log_Should_SupportUserActionWithoutGroup()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var log = new Log(LogType.Login, "192.168.1.1", user);

        // Assert
        log.User.Should().NotBeNull();
        log.UserId.Should().Be(user.Id);
        log.Group.Should().BeNull();
        log.GroupId.Should().BeNull();
    }

    [Fact]
    public void Log_Should_SupportGroupActionWithUser()
    {
        // Arrange
        var user = CreateTestUser();
        var group = CreateTestGroup();

        // Act
        var log = new Log(LogType.JoinGroup, "192.168.1.1", user, group);

        // Assert
        log.User.Should().NotBeNull();
        log.Group.Should().NotBeNull();
        log.UserId.Should().Be(user.Id);
        log.GroupId.Should().Be(group.Id);
    }

    [Fact]
    public void Log_Should_SupportCompetitionAction()
    {
        // Arrange
        var user = CreateTestUser();
        var competition = CreateTestCompetition();

        // Act
        var log = new Log(LogType.JoinCompetition, "192.168.1.1", user, null, competition);

        // Assert
        log.User.Should().NotBeNull();
        log.Competition.Should().NotBeNull();
        log.CompetitionId.Should().Be(competition.Id);
    }

    // Helper methods
    private static User CreateTestUser()
    {
        return new User("Test User", "test@example.com", "12345");
    }

    private static Group CreateTestGroup()
    {
        var leader = CreateTestUser();
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
