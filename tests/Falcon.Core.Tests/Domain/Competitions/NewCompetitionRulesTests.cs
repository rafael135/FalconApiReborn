using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Competitions.Rules;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Competitions;

public class NewCompetitionRulesTests
{
    [Fact]
    public void EndInscriptionsCannotBeBeforeStartRule_Should_BeBroken_WhenEndBeforeStart()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var end = start.AddDays(-1);
        var rule = new EndInscriptionsCannotBeBeforeStartRule(start, end);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("inscrições");
    }

    [Fact]
    public void EndInscriptionsCannotBeBeforeStartRule_Should_NotBeBroken_WhenEndAfterStart()
    {
        // Arrange
        var start = DateTime.UtcNow;
        var end = start.AddDays(7);
        var rule = new EndInscriptionsCannotBeBeforeStartRule(start, end);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }

    [Fact]
    public void OnlyTemplateCanBePromotedRule_Should_BeBroken_WhenNotTemplate()
    {
        // Arrange
        var rule = new OnlyTemplateCanBePromotedRule(CompetitionStatus.Pending);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("competições modelo");
    }

    [Fact]
    public void OnlyTemplateCanBePromotedRule_Should_NotBeBroken_WhenTemplate()
    {
        // Arrange
        var rule = new OnlyTemplateCanBePromotedRule(CompetitionStatus.ModelTemplate);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }

    [Fact]
    public void CompetitionMustBePendingToOpenInscriptionsRule_Should_BeBroken_WhenNotPending()
    {
        // Arrange
        var rule = new CompetitionMustBePendingToOpenInscriptionsRule(CompetitionStatus.ModelTemplate);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("Pendente");
    }

    [Fact]
    public void CompetitionMustBePendingToOpenInscriptionsRule_Should_NotBeBroken_WhenPending()
    {
        // Arrange
        var rule = new CompetitionMustBePendingToOpenInscriptionsRule(CompetitionStatus.Pending);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }

    [Fact]
    public void ExerciseCannotBeAddedTwiceRule_Should_BeBroken_WhenAlreadyAdded()
    {
        // Arrange
        var rule = new ExerciseCannotBeAddedTwiceRule(true);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeTrue();
        rule.Message.Should().Contain("já foi adicionado");
    }

    [Fact]
    public void ExerciseCannotBeAddedTwiceRule_Should_NotBeBroken_WhenNotAdded()
    {
        // Arrange
        var rule = new ExerciseCannotBeAddedTwiceRule(false);

        // Act
        var isBroken = rule.IsBroken();

        // Assert
        isBroken.Should().BeFalse();
    }
}
