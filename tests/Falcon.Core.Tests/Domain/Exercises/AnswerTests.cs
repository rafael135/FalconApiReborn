using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Users;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Exercises;

public class AnswerTests
{
    [Fact]
    public void Constructor_Should_CreateAnswer_WithValidParameters()
    {
        // Arrange
        var user = CreateTestUser();
        var content = "Esta é uma resposta detalhada para a sua pergunta.";

        // Act
        var answer = new Answer(user, content);

        // Assert
        answer.Should().NotBeNull();
        answer.User.Should().Be(user);
        answer.UserId.Should().Be(user.Id);
        answer.Content.Should().Be(content);
        answer.Question.Should().BeNull();
        answer.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenUserIsNull()
    {
        // Act
        Action act = () => new Answer(null!, "content");

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("user");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenContentIsInvalid(string? invalidContent)
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        Action act = () => new Answer(user, invalidContent!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*conteúdo*resposta*vazio*");
    }

    [Fact]
    public void UpdateContent_Should_UpdateContent_WithValidContent()
    {
        // Arrange
        var answer = CreateTestAnswer();
        var newContent = "Esta é uma resposta atualizada com mais detalhes.";

        // Act
        answer.UpdateContent(newContent);

        // Assert
        answer.Content.Should().Be(newContent);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateContent_Should_ThrowArgumentException_WhenContentIsInvalid(string? invalidContent)
    {
        // Arrange
        var answer = CreateTestAnswer();

        // Act
        Action act = () => answer.UpdateContent(invalidContent!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*conteúdo*resposta*vazio*");
    }

    [Fact]
    public void UpdateContent_Should_AllowMultipleUpdates()
    {
        // Arrange
        var answer = CreateTestAnswer();
        var firstUpdate = "Primeira atualização";
        var secondUpdate = "Segunda atualização";

        // Act
        answer.UpdateContent(firstUpdate);
        answer.UpdateContent(secondUpdate);

        // Assert
        answer.Content.Should().Be(secondUpdate);
    }

    [Fact]
    public void Answer_Should_SupportLongContent()
    {
        // Arrange
        var user = CreateTestUser();
        var longContent = new string('A', 5000); // 5000 characters

        // Act
        var answer = new Answer(user, longContent);

        // Assert
        answer.Content.Should().HaveLength(5000);
        answer.Content.Should().Be(longContent);
    }

    [Fact]
    public void Answer_Should_PreserveOriginalContentBeforeUpdate()
    {
        // Arrange
        var user = CreateTestUser();
        var originalContent = "Conteúdo original";
        var answer = new Answer(user, originalContent);

        // Act & Assert
        answer.Content.Should().Be(originalContent);
        
        answer.UpdateContent("Novo conteúdo");
        answer.Content.Should().NotBe(originalContent);
    }

    [Fact]
    public void Answer_Should_MaintainUserReference()
    {
        // Arrange
        var user = CreateTestUser();
        var answer = new Answer(user, "Initial content");

        // Act
        answer.UpdateContent("Updated content");

        // Assert
        answer.User.Should().Be(user);
        answer.UserId.Should().Be(user.Id);
    }

    [Fact]
    public void Answer_Should_MaintainCreatedAt_AfterUpdate()
    {
        // Arrange
        var answer = CreateTestAnswer();
        var originalCreatedAt = answer.CreatedAt;

        // Act
        Thread.Sleep(10); // Small delay to ensure time difference
        answer.UpdateContent("Updated content");

        // Assert
        answer.CreatedAt.Should().Be(originalCreatedAt);
    }

    [Fact]
    public void Constructor_Should_AcceptContentWithSpecialCharacters()
    {
        // Arrange
        var user = CreateTestUser();
        var content = "Resposta com caracteres especiais: @#$%&*()[]{}!?\nNova linha\tTab";

        // Act
        var answer = new Answer(user, content);

        // Assert
        answer.Content.Should().Be(content);
    }

    [Fact]
    public void Constructor_Should_AcceptContentWithEmojis()
    {
        // Arrange
        var user = CreateTestUser();
        var content = "Resposta com emojis 😊 🎉 👍";

        // Act
        var answer = new Answer(user, content);

        // Assert
        answer.Content.Should().Be(content);
    }

    // Helper methods
    private static Answer CreateTestAnswer()
    {
        return new Answer(CreateTestUser(), "Test answer content");
    }

    private static User CreateTestUser()
    {
        return new User("Test User", "test@example.com", "12345");
    }
}
