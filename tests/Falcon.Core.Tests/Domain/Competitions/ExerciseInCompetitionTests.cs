using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Exercises;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Competitions;

public class ExerciseInCompetitionTests
{
    [Fact]
    public void Constructor_Should_CreateExerciseInCompetition_WithValidParameters()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var exercise = CreateTestExercise();

        // Act
        var exerciseInCompetition = new ExerciseInCompetition(competition, exercise);

        // Assert
        exerciseInCompetition.Should().NotBeNull();
        exerciseInCompetition.Competition.Should().Be(competition);
        exerciseInCompetition.CompetitionId.Should().Be(competition.Id);
        exerciseInCompetition.Exercise.Should().Be(exercise);
        exerciseInCompetition.ExerciseId.Should().Be(exercise.Id);
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenCompetitionIsNull()
    {
        // Arrange
        var exercise = CreateTestExercise();

        // Act
        Action act = () => new ExerciseInCompetition(null!, exercise);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("competition");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenExerciseIsNull()
    {
        // Arrange
        var competition = CreateTestCompetition();

        // Act
        Action act = () => new ExerciseInCompetition(competition, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("exercise");
    }

    [Fact]
    public void Constructor_Should_ProperlySetCompetitionId()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var exercise = CreateTestExercise();

        // Act
        var exerciseInCompetition = new ExerciseInCompetition(competition, exercise);

        // Assert
        exerciseInCompetition.CompetitionId.Should().Be(competition.Id);
    }

    [Fact]
    public void Constructor_Should_ProperlySetExerciseId()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var exercise = CreateTestExercise();

        // Act
        var exerciseInCompetition = new ExerciseInCompetition(competition, exercise);

        // Assert
        exerciseInCompetition.ExerciseId.Should().Be(exercise.Id);
    }

    [Fact]
    public void ExerciseInCompetition_Should_SupportMultipleExercisesInSameCompetition()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var exercise1 = CreateTestExercise("Exercise 1");
        var exercise2 = CreateTestExercise("Exercise 2");

        // Act
        var link1 = new ExerciseInCompetition(competition, exercise1);
        var link2 = new ExerciseInCompetition(competition, exercise2);

        // Assert
        link1.CompetitionId.Should().Be(competition.Id);
        link2.CompetitionId.Should().Be(competition.Id);
    }

    [Fact]
    public void ExerciseInCompetition_Should_SupportSameExerciseInMultipleCompetitions()
    {
        // Arrange
        var competition1 = CreateTestCompetition("Competition 1");
        var competition2 = CreateTestCompetition("Competition 2");
        var exercise = CreateTestExercise();

        // Act
        var link1 = new ExerciseInCompetition(competition1, exercise);
        var link2 = new ExerciseInCompetition(competition2, exercise);

        // Assert
        link1.ExerciseId.Should().Be(exercise.Id);
        link2.ExerciseId.Should().Be(exercise.Id);
    }

    [Fact]
    public void Constructor_Should_MaintainReferencesAfterCreation()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var exercise = CreateTestExercise();

        // Act
        var exerciseInCompetition = new ExerciseInCompetition(competition, exercise);

        // Assert
        exerciseInCompetition.Competition.Should().NotBeNull();
        exerciseInCompetition.Competition.Should().BeSameAs(competition);
        exerciseInCompetition.Exercise.Should().NotBeNull();
        exerciseInCompetition.Exercise.Should().BeSameAs(exercise);
    }

    // Helper methods
    private static Competition CreateTestCompetition()
    {
        return CreateTestCompetition("Test Competition");
    }

    private static Competition CreateTestCompetition(string name)
    {
        return Competition.CreateTemplate(
            name,
            "Description",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2),
            DateTime.UtcNow.AddDays(3));
    }

    private static Exercise CreateTestExercise()
    {
        return CreateTestExercise("Test Exercise");
    }

    private static Exercise CreateTestExercise(string title)
    {
        return new Exercise(
            title,
            "Description",
            1,
            TimeSpan.FromMinutes(30));
    }
}
