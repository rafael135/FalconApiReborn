using Falcon.Core.Domain.Exercises;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Exercises;

public class ExerciseInputTests
{
    [Fact]
    public void Constructor_Should_CreateExerciseInput_WithValidContent()
    {
        // Arrange
        var inputContent = "5 10";

        // Act
        var exerciseInput = new ExerciseInput(inputContent);

        // Assert
        exerciseInput.Should().NotBeNull();
        exerciseInput.InputContent.Should().Be(inputContent);
        exerciseInput.Output.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenContentIsInvalid(string? invalidContent)
    {
        // Act
        Action act = () => new ExerciseInput(invalidContent!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*entrada*obrigatório*");
    }

    [Fact]
    public void SetExercise_Should_SetExerciseId()
    {
        // Arrange
        var exerciseInput = new ExerciseInput("test input");
        var exercise = CreateTestExercise();

        // Act
        exerciseInput.SetExercise(exercise);

        // Assert
        exerciseInput.ExerciseId.Should().Be(exercise.Id);
    }

    [Fact]
    public void SetExercise_Should_ThrowArgumentNullException_WhenExerciseIsNull()
    {
        // Arrange
        var exerciseInput = new ExerciseInput("test input");

        // Act
        Action act = () => exerciseInput.SetExercise(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("exercise");
    }

    [Fact]
    public void Constructor_Should_AcceptNumericInput()
    {
        // Arrange
        var inputContent = "42";

        // Act
        var exerciseInput = new ExerciseInput(inputContent);

        // Assert
        exerciseInput.InputContent.Should().Be(inputContent);
    }

    [Fact]
    public void Constructor_Should_AcceptMultilineInput()
    {
        // Arrange
        var inputContent = "5\n10\n15\n20\n25";

        // Act
        var exerciseInput = new ExerciseInput(inputContent);

        // Assert
        exerciseInput.InputContent.Should().Be(inputContent);
    }

    [Fact]
    public void Constructor_Should_AcceptComplexInput()
    {
        // Arrange
        var inputContent = "10 20 30\nabc def\n123.456";

        // Act
        var exerciseInput = new ExerciseInput(inputContent);

        // Assert
        exerciseInput.InputContent.Should().Be(inputContent);
    }

    [Fact]
    public void Constructor_Should_AcceptLargeInput()
    {
        // Arrange
        var inputContent = new string('A', 10000);

        // Act
        var exerciseInput = new ExerciseInput(inputContent);

        // Assert
        exerciseInput.InputContent.Should().HaveLength(10000);
    }

    [Fact]
    public void SetExercise_Should_AllowChangingExercise()
    {
        // Arrange
        var exerciseInput = new ExerciseInput("test input");
        var exercise1 = CreateTestExercise();
        var exercise2 = CreateTestExercise();

        // Act
        exerciseInput.SetExercise(exercise1);
        var firstExerciseId = exerciseInput.ExerciseId;
        
        exerciseInput.SetExercise(exercise2);

        // Assert
        exerciseInput.ExerciseId.Should().Be(exercise2.Id);
    }

    // Helper methods
    private static Exercise CreateTestExercise()
    {
        return new Exercise(
            "Test Exercise",
            "Description",
            1,
            TimeSpan.FromMinutes(30));
    }
}
