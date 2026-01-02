using Falcon.Core.Domain.Groups.Rules;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Groups;

public class GroupCannotHaveMoreThanMaxMembersRuleTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void IsBroken_Should_ReturnFalse_WhenMembersCountIsWithinLimit(int memberCount)
    {
        // Arrange
        var rule = new GroupCannotHaveMoreThanMaxMembersRule(memberCount);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }

    [Theory]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(10)]
    public void IsBroken_Should_ReturnTrue_WhenMembersCountExceedsLimit(int memberCount)
    {
        // Arrange
        var rule = new GroupCannotHaveMoreThanMaxMembersRule(memberCount);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
    }

    [Fact]
    public void Message_Should_IndicateMaxMembersLimit()
    {
        // Arrange
        var rule = new GroupCannotHaveMoreThanMaxMembersRule(5);

        // Act
        var message = rule.Message;

        // Assert
        message.Should().Contain("3 membros");
    }
}
