using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Exceptions;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Competitions;

public class CompetitionTests
{
    [Fact]
    public void CreateTemplate_Should_CreateCompetition_WithValidParameters()
    {
        // Arrange
        var name = "Test Competition";
        var description = "Test Description";
        var startInscriptions = DateTime.UtcNow;
        var endInscriptions = startInscriptions.AddDays(7);
        var startTime = endInscriptions.AddDays(1);

        // Act
        var competition = Competition.CreateTemplate(
            name,
            description,
            startInscriptions,
            endInscriptions,
            startTime
        );

        // Assert
        competition.Should().NotBeNull();
        competition.Name.Should().Be(name);
        competition.Description.Should().Be(description);
        competition.StartInscriptions.Should().Be(startInscriptions);
        competition.EndInscriptions.Should().Be(endInscriptions);
        competition.StartTime.Should().Be(startTime);
        competition.Status.Should().Be(CompetitionStatus.ModelTemplate);
    }

    [Fact]
    public void CreateTemplate_Should_ThrowDomainException_WhenEndInscriptionsBeforeStart()
    {
        // Arrange
        var startInscriptions = DateTime.UtcNow;
        var endInscriptions = startInscriptions.AddDays(-1); // Before start
        var startTime = DateTime.UtcNow.AddDays(7);

        // Act
        Action act = () => Competition.CreateTemplate(
            "Test",
            "Description",
            startInscriptions,
            endInscriptions,
            startTime
        );

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*inscrições*");
    }

    [Fact]
    public void IsInscriptionOpen_Should_ReturnTrue_WhenWithinInscriptionPeriod()
    {
        // Arrange
        var startInscriptions = DateTime.UtcNow.AddHours(-1);
        var endInscriptions = DateTime.UtcNow.AddHours(1);
        var competition = Competition.CreateTemplate(
            "Test",
            "Description",
            startInscriptions,
            endInscriptions,
            DateTime.UtcNow.AddDays(1)
        );

        // Act & Assert
        competition.IsInscriptionOpen.Should().BeTrue();
    }

    [Fact]
    public void IsInscriptionOpen_Should_ReturnFalse_WhenOutsideInscriptionPeriod()
    {
        // Arrange
        var startInscriptions = DateTime.UtcNow.AddDays(-10);
        var endInscriptions = DateTime.UtcNow.AddDays(-5);
        var competition = Competition.CreateTemplate(
            "Test",
            "Description",
            startInscriptions,
            endInscriptions,
            DateTime.UtcNow.AddDays(1)
        );

        // Act & Assert
        competition.IsInscriptionOpen.Should().BeFalse();
    }

    [Fact]
    public void CreateTemplate_Should_SetDefaultValues()
    {
        // Arrange & Act
        var competition = CreateTestCompetition();

        // Assert
        competition.MaxMembers.Should().Be(3);
        competition.MaxExercises.Should().Be(3);
        competition.Duration.Should().Be(TimeSpan.FromHours(2));
        competition.SubmissionPenalty.Should().Be(TimeSpan.FromMinutes(10));
    }

    [Fact]
    public void PromoteToCompetition_Should_ThrowException_WhenNotTemplate()
    {
        // Arrange
        var competition = CreateTestCompetition();
        competition.PromoteToCompetition(3, 3, 1024, TimeSpan.FromHours(2), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        // Act - Try to promote again
        Action act = () => competition.PromoteToCompetition(3, 3, 1024, TimeSpan.FromHours(2), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*competições modelo*");
    }

    [Fact]
    public void OpenInscriptions_Should_ChangeStatusToOpenInscriptions()
    {
        // Arrange
        var competition = CreateTestCompetition();
        competition.PromoteToCompetition(3, 3, 1024, TimeSpan.FromHours(2), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        // Act
        competition.OpenInscriptions();

        // Assert
        competition.Status.Should().Be(CompetitionStatus.OpenInscriptions);
    }

    [Fact]
    public void OpenInscriptions_Should_ThrowException_WhenNotPending()
    {
        // Arrange
        var competition = CreateTestCompetition(); // Status is ModelTemplate

        // Act
        Action act = () => competition.OpenInscriptions();

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*Pendente*");
    }

    [Fact]
    public void Start_Should_ChangeStatusToOngoing()
    {
        // Arrange
        var competition = CreateTestCompetition();
        competition.PromoteToCompetition(3, 3, 1024, TimeSpan.FromHours(2), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        // Act
        competition.Start();

        // Assert
        competition.Status.Should().Be(CompetitionStatus.Ongoing);
    }

    [Fact]
    public void Finish_Should_ChangeStatusToFinished()
    {
        // Arrange
        var competition = CreateTestCompetition();
        competition.PromoteToCompetition(3, 3, 1024, TimeSpan.FromHours(2), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromMinutes(10));

        // Act
        competition.Finish();

        // Assert
        competition.Status.Should().Be(CompetitionStatus.Finished);
    }

    [Fact]
    public void AddExercise_Should_AddExerciseToCompetition()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var exercise = CreateTestExercise();

        // Act
        competition.AddExercise(exercise);

        // Assert
        competition.ExercisesInCompetition.Should().HaveCount(1);
        competition.ExercisesInCompetition.First().ExerciseId.Should().Be(exercise.Id);
    }

    [Fact]
    public void AddExercise_Should_ThrowException_WhenExerciseAlreadyAdded()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var exercise = CreateTestExercise();
        competition.AddExercise(exercise);

        // Act
        Action act = () => competition.AddExercise(exercise);

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*já foi adicionado*");
    }

    [Fact]
    public void AddExercise_Should_IncrementExerciseCount()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var exercise = CreateTestExercise();

        // Act
        competition.PromoteToCompetition(3, 3, 1024, TimeSpan.FromHours(2), TimeSpan.Zero, TimeSpan.Zero, TimeSpan.FromMinutes(10));
        competition.AddExercise(exercise);

        // Assert
        competition.ExercisesInCompetition.Should().HaveCount(1);
    }

    // Helper methods
    private static Competition CreateTestCompetition()
    {
        var startInscriptions = DateTime.UtcNow.AddDays(-10);
        var endInscriptions = startInscriptions.AddDays(7);
        var startTime = endInscriptions.AddDays(1);

        return Competition.CreateTemplate(
            "Test Competition",
            "Test Description",
            startInscriptions,
            endInscriptions,
            startTime
        );
    }

    private static Exercise CreateTestExercise()
    {
        return new Exercise(
            "Test Exercise",
            "Description",
            1,
            TimeSpan.FromMinutes(30));
    }
}
