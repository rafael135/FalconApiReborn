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
}
