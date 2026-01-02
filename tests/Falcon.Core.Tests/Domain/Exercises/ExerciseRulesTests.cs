using Falcon.Core.Domain.Exercises.Rules;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Exercises;

public class ExerciseRulesTests
{
    [Fact]
    public void ExerciseMustHaveValidUuidRule_Should_BeBroken_WhenUuidIsNull()
    {
        // Arrange
        var rule = new ExerciseMustHaveValidUuidRule(null!);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("UUID");
        rule.Message.Should().Contain("obrigatório");
    }

    [Fact]
    public void ExerciseMustHaveValidUuidRule_Should_BeBroken_WhenUuidIsEmpty()
    {
        // Arrange
        var rule = new ExerciseMustHaveValidUuidRule("");

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
    }

    [Fact]
    public void ExerciseMustHaveValidUuidRule_Should_BeBroken_WhenUuidIsWhitespace()
    {
        // Arrange
        var rule = new ExerciseMustHaveValidUuidRule("   ");

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
    }

    [Fact]
    public void ExerciseMustHaveValidUuidRule_Should_NotBeBroken_WhenUuidIsValid()
    {
        // Arrange
        var rule = new ExerciseMustHaveValidUuidRule("550e8400-e29b-41d4-a716-446655440000");

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }
}
