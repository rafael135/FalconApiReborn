using Falcon.Core.Domain.Competitions.Rules;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Competitions;

public class CompetitionRankingRulesTests
{
    [Fact]
    public void PointsCannotBeNegativeRule_Should_BeBroken_WhenPointsAreNegative()
    {
        // Arrange
        var rule = new PointsCannotBeNegativeRule(-10);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("pontos");
        rule.Message.Should().Contain("negativos");
    }

    [Fact]
    public void PointsCannotBeNegativeRule_Should_NotBeBroken_WhenPointsArePositive()
    {
        // Arrange
        var rule = new PointsCannotBeNegativeRule(100);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }

    [Fact]
    public void PointsCannotBeNegativeRule_Should_NotBeBroken_WhenPointsAreZero()
    {
        // Arrange
        var rule = new PointsCannotBeNegativeRule(0);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }

    [Fact]
    public void PenaltyCannotBeNegativeRule_Should_BeBroken_WhenPenaltyIsNegative()
    {
        // Arrange
        var rule = new PenaltyCannotBeNegativeRule(-5);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("penalidade");
        rule.Message.Should().Contain("negativa");
    }

    [Fact]
    public void PenaltyCannotBeNegativeRule_Should_NotBeBroken_WhenPenaltyIsPositive()
    {
        // Arrange
        var rule = new PenaltyCannotBeNegativeRule(10);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }

    [Fact]
    public void RankOrderMustBePositiveRule_Should_BeBroken_WhenOrderIsZero()
    {
        // Arrange
        var rule = new RankOrderMustBePositiveRule(0);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("ranking");
        rule.Message.Should().Contain("maior que 0");
    }

    [Fact]
    public void RankOrderMustBePositiveRule_Should_BeBroken_WhenOrderIsNegative()
    {
        // Arrange
        var rule = new RankOrderMustBePositiveRule(-1);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
    }

    [Fact]
    public void RankOrderMustBePositiveRule_Should_NotBeBroken_WhenOrderIsPositive()
    {
        // Arrange
        var rule = new RankOrderMustBePositiveRule(1);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }
}
