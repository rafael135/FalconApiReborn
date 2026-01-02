using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Users;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Users;

public class UserTests
{
    [Fact]
    public void Constructor_Should_CreateUser_WithValidParameters()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@test.com";
        var ra = "123456";
        var joinYear = 2024;
        var department = "Computer Science";

        // Act
        var user = new User(name, email, ra, joinYear, department);

        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.RA.Should().Be(ra);
        user.JoinYear.Should().Be(joinYear);
        user.Department.Should().Be(department);
        user.UserName.Should().Be(email); // UserName is set to Email
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenNameIsInvalid(string? invalidName)
    {
        // Act
        Action act = () => new User(invalidName!, "email@test.com", "123456", 2024, "CS");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*inválido*");
    }

    [Fact]
    public void Constructor_Should_AcceptEmail_WithoutValidation()
    {
        // Note: Email validation is handled by ASP.NET Identity, not by User entity
        // Act
        var user = new User("John Doe", "email@test.com", "123456", 2024, "CS");

        // Assert
        user.Email.Should().Be("email@test.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenRAIsInvalid(string? invalidRA)
    {
        // Act
        Action act = () => new User("John Doe", "email@test.com", invalidRA!, 2024, "CS");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*inválido*");
    }

    [Fact]
    public void EmailConfirmed_Should_DefaultToFalse()
    {
        // Arrange & Act
        var user = CreateTestUser();

        // Assert
        user.EmailConfirmed.Should().BeFalse();
    }

    [Fact]
    public void User_Should_HaveGroupIdProperty()
    {
        // Arrange & Act
        var user = CreateTestUser();

        // Assert
        user.GroupId.Should().BeNull();
    }

    [Fact]
    public void UpdateLastLogin_Should_UpdateLastLoggedAt()
    {
        // Arrange
        var user = CreateTestUser();
        var originalLastLoggedAt = user.LastLoggedAt;

        // Act
        Thread.Sleep(10);
        user.UpdateLastLogin();

        // Assert
        user.LastLoggedAt.Should().BeAfter(originalLastLoggedAt);
        user.LastLoggedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateLastLogin_Should_AllowMultipleUpdates()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.UpdateLastLogin();
        var firstLogin = user.LastLoggedAt;
        
        Thread.Sleep(10);
        user.UpdateLastLogin();
        var secondLogin = user.LastLoggedAt;

        // Assert
        secondLogin.Should().BeAfter(firstLogin);
    }

    [Fact]
    public void AssignGroup_Should_SetGroupAndGroupId()
    {
        // Arrange
        var user = CreateTestUser();
        var group = CreateTestGroup();

        // Act
        user.AssignGroup(group);

        // Assert
        user.Group.Should().Be(group);
        user.GroupId.Should().Be(group.Id);
    }

    [Fact]
    public void AssignGroup_Should_AllowChangingGroup()
    {
        // Arrange
        var user = CreateTestUser();
        var group1 = CreateTestGroup("Group 1");
        var group2 = CreateTestGroup("Group 2");

        // Act
        user.AssignGroup(group1);
        user.AssignGroup(group2);

        // Assert
        user.Group.Should().Be(group2);
        user.GroupId.Should().Be(group2.Id);
    }

    [Fact]
    public void LeaveGroup_Should_SetGroupToNull()
    {
        // Arrange
        var user = CreateTestUser();
        var group = CreateTestGroup();
        user.AssignGroup(group);

        // Act
        user.LeaveGroup();

        // Assert
        user.Group.Should().BeNull();
        user.GroupId.Should().BeNull();
    }

    [Fact]
    public void LeaveGroup_Should_BeIdempotent()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        user.LeaveGroup();
        user.LeaveGroup();
        user.LeaveGroup();

        // Assert
        user.Group.Should().BeNull();
        user.GroupId.Should().BeNull();
    }

    [Fact]
    public void PhoneNumberConfirmed_Should_DefaultToFalse()
    {
        // Arrange & Act
        var user = CreateTestUser();

        // Assert
        user.PhoneNumberConfirmed.Should().BeFalse();
    }

    [Fact]
    public void TwoFactorEnabled_Should_DefaultToFalse()
    {
        // Arrange & Act
        var user = CreateTestUser();

        // Assert
        user.TwoFactorEnabled.Should().BeFalse();
    }

    [Fact]
    public void UserName_Should_AlwaysEqualEmail()
    {
        // Arrange
        var email = "test@example.com";

        // Act
        var user = new User("Test User", email, "123456");

        // Assert
        user.UserName.Should().Be(email);
    }

    // Helper methods
    private static User CreateTestUser()
    {
        return new User(
            "Test User",
            "test@example.com",
            "123456",
            2024,
            "Computer Science"
        );
    }

    private static Group CreateTestGroup()
    {
        return CreateTestGroup("Test Group");
    }

    private static Group CreateTestGroup(string name)
    {
        var leader = new User("Leader", "leader@test.com", "99999");
        return new Group(name, leader);
    }
}
