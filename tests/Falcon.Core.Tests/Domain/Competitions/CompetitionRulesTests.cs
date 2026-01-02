using Falcon.Core.Domain.Competitions.Rules;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Competitions;

public class CompetitionRulesTests
{
    [Fact]
    public void InscriptionsMustBeOpenRule_Should_BeBroken_WhenInscriptionsClosed()
    {
        // Arrange
        var rule = new InscriptionsMustBeOpenRule(false);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("inscrições");
    }

    [Fact]
    public void InscriptionsMustBeOpenRule_Should_NotBeBroken_WhenInscriptionsOpen()
    {
        // Arrange
        var rule = new InscriptionsMustBeOpenRule(true);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }

    [Fact]
    public void GroupCannotBeAlreadyRegisteredRule_Should_BeBroken_WhenGroupAlreadyRegistered()
    {
        // Arrange
        var rule = new GroupCannotBeAlreadyRegisteredRule(true);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("registrado");
    }

    [Fact]
    public void GroupCannotBeAlreadyRegisteredRule_Should_NotBeBroken_WhenGroupNotRegistered()
    {
        // Arrange
        var rule = new GroupCannotBeAlreadyRegisteredRule(false);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }

    [Theory]
    [InlineData(1, 1, false)] // Exactly 1 member required and has 1
    [InlineData(2, 2, false)] // Exactly 2 members required and has 2
    [InlineData(3, 3, false)] // Exactly 3 members required and has 3
    public void GroupMustHaveRequiredMembersRule_Should_NotBeBroken_WhenHasExactMembers(
        int actualMembers,
        int requiredMembers,
        bool expectedBroken)
    {
        // Arrange - Rule requires EXACTLY the specified number (uses != operator)
        var rule = new GroupMustHaveRequiredMembersRule(actualMembers, requiredMembers);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().Be(expectedBroken);
    }

    [Theory]
    [InlineData(1, 2, true)] // Has 1 but needs 2
    [InlineData(0, 1, true)] // Has 0 but needs 1
    [InlineData(2, 3, true)] // Has 2 but needs 3
    [InlineData(2, 1, true)] // Has 2 but needs 1 (too many also fails!)
    [InlineData(4, 3, true)] // Has 4 but needs 3 (too many)
    public void GroupMustHaveRequiredMembersRule_Should_BeBroken_WhenNotExactMembers(
        int actualMembers,
        int requiredMembers,
        bool expectedBroken)
    {
        // Arrange - Rule uses != so both too few AND too many will fail
        var rule = new GroupMustHaveRequiredMembersRule(actualMembers, requiredMembers);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().Be(expectedBroken);
        if (expectedBroken)
        {
            rule.Message.Should().Contain("exatamente"); // Portuguese: "exactly"
        }
    }
}
