using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Shared.Exceptions;
using Falcon.Core.Domain.Users;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Competitions;

public class CompetitionRankingTests
{
    [Fact]
    public void Constructor_Should_CreateRanking_WithValidParameters()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var group = CreateTestGroup();

        // Act
        var ranking = new CompetitionRanking(competition, group);

        // Assert
        ranking.Should().NotBeNull();
        ranking.Competition.Should().Be(competition);
        ranking.CompetitionId.Should().Be(competition.Id);
        ranking.Group.Should().Be(group);
        ranking.GroupId.Should().Be(group.Id);
        ranking.Points.Should().Be(0);
        ranking.Penalty.Should().Be(0);
        ranking.RankOrder.Should().Be(0);
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenCompetitionIsNull()
    {
        // Arrange
        var group = CreateTestGroup();

        // Act
        Action act = () => new CompetitionRanking(null!, group);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("competition");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenGroupIsNull()
    {
        // Arrange
        var competition = CreateTestCompetition();

        // Act
        Action act = () => new CompetitionRanking(competition, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("group");
    }

    [Fact]
    public void UpdatePoints_Should_UpdatePoints_WithValidPositivePoints()
    {
        // Arrange
        var ranking = CreateTestRanking();
        var newPoints = 150.5;

        // Act
        ranking.UpdatePoints(newPoints);

        // Assert
        ranking.Points.Should().Be(newPoints);
    }

    [Fact]
    public void UpdatePoints_Should_UpdatePoints_WithZeroPoints()
    {
        // Arrange
        var ranking = CreateTestRanking();
        ranking.UpdatePoints(100);

        // Act
        ranking.UpdatePoints(0);

        // Assert
        ranking.Points.Should().Be(0);
    }

    [Fact]
    public void UpdatePoints_Should_ThrowBusinessRuleException_WhenPointsAreNegative()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act
        Action act = () => ranking.UpdatePoints(-10);

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*pontos*negativos*");
    }

    [Fact]
    public void UpdatePoints_Should_AllowMultipleUpdates()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act
        ranking.UpdatePoints(50);
        ranking.UpdatePoints(100);
        ranking.UpdatePoints(75);

        // Assert
        ranking.Points.Should().Be(75);
    }

    [Fact]
    public void AddPenalty_Should_AddPenalty_WithValidPositivePenalty()
    {
        // Arrange
        var ranking = CreateTestRanking();
        var penalty = 15.5;

        // Act
        ranking.AddPenalty(penalty);

        // Assert
        ranking.Penalty.Should().Be(penalty);
    }

    [Fact]
    public void AddPenalty_Should_AccumulatePenalties()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act
        ranking.AddPenalty(10);
        ranking.AddPenalty(5);
        ranking.AddPenalty(3.5);

        // Assert
        ranking.Penalty.Should().Be(18.5);
    }

    [Fact]
    public void AddPenalty_Should_AcceptZeroPenalty()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act
        ranking.AddPenalty(0);

        // Assert
        ranking.Penalty.Should().Be(0);
    }

    [Fact]
    public void AddPenalty_Should_ThrowBusinessRuleException_WhenPenaltyIsNegative()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act
        Action act = () => ranking.AddPenalty(-5);

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*penalidade*negativa*");
    }

    [Fact]
    public void UpdateRankOrder_Should_UpdateRankOrder_WithValidPositiveOrder()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act
        ranking.UpdateRankOrder(5);

        // Assert
        ranking.RankOrder.Should().Be(5);
    }

    [Fact]
    public void UpdateRankOrder_Should_ThrowBusinessRuleException_WhenOrderIsZero()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act
        Action act = () => ranking.UpdateRankOrder(0);

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*ranking*maior que 0*");
    }

    [Fact]
    public void UpdateRankOrder_Should_ThrowBusinessRuleException_WhenOrderIsNegative()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act
        Action act = () => ranking.UpdateRankOrder(-1);

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*ranking*maior que 0*");
    }

    [Fact]
    public void UpdateRankOrder_Should_AllowChangingOrder()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act
        ranking.UpdateRankOrder(10);
        ranking.UpdateRankOrder(5);
        ranking.UpdateRankOrder(1);

        // Assert
        ranking.RankOrder.Should().Be(1);
    }

    [Fact]
    public void Ranking_Should_SupportCompleteScenario()
    {
        // Arrange
        var ranking = CreateTestRanking();

        // Act - Simulate competition progress
        ranking.UpdatePoints(10); // First correct submission
        ranking.UpdateRankOrder(1); // Now first place
        
        ranking.AddPenalty(5); // Late submission penalty
        
        ranking.UpdatePoints(20); // Second correct submission
        ranking.UpdateRankOrder(2); // Dropped to second place

        // Assert
        ranking.Points.Should().Be(20);
        ranking.Penalty.Should().Be(5);
        ranking.RankOrder.Should().Be(2);
    }

    [Fact]
    public void Ranking_Should_MaintainGroupAndCompetitionReferences()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var group = CreateTestGroup();
        var ranking = new CompetitionRanking(competition, group);

        // Act
        ranking.UpdatePoints(50);
        ranking.AddPenalty(10);
        ranking.UpdateRankOrder(3);

        // Assert
        ranking.Competition.Should().Be(competition);
        ranking.CompetitionId.Should().Be(competition.Id);
        ranking.Group.Should().Be(group);
        ranking.GroupId.Should().Be(group.Id);
    }

    // Helper methods
    private static CompetitionRanking CreateTestRanking()
    {
        return new CompetitionRanking(CreateTestCompetition(), CreateTestGroup());
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

    private static Group CreateTestGroup()
    {
        var leader = new User("Leader", "leader@test.com", "12345");
        return new Group("Test Group", leader);
    }
}
