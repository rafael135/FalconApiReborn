using Falcon.Core.Domain.Exercises;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Exercises;

public class ExerciseOutputTests
{
    [Fact]
    public void Constructor_Should_CreateExerciseOutput_WithValidParameters()
    {
        // Arrange
        var outputContent = "15";
        var exercise = CreateTestExercise();

        // Act
        var exerciseOutput = new ExerciseOutput(outputContent, exercise);

        // Assert
        exerciseOutput.Should().NotBeNull();
        exerciseOutput.OutputContent.Should().Be(outputContent);
        exerciseOutput.Exercise.Should().Be(exercise);
        exerciseOutput.ExerciseId.Should().Be(exercise.Id);
        exerciseOutput.ExerciseInput.Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenOutputContentIsInvalid(string? invalidContent)
    {
        // Arrange
        var exercise = CreateTestExercise();

        // Act
        Action act = () => new ExerciseOutput(invalidContent!, exercise);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*saída*obrigatório*");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenExerciseIsNull()
    {
        // Act
        Action act = () => new ExerciseOutput("output", null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Constructor_Should_ProperlySetExerciseId()
    {
        // Arrange
        var exercise = CreateTestExercise();

        // Act
        var exerciseOutput = new ExerciseOutput("output", exercise);

        // Assert
        exerciseOutput.ExerciseId.Should().Be(exercise.Id);
    }

    [Fact]
    public void Constructor_Should_AcceptNumericOutput()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var outputContent = "42";

        // Act
        var exerciseOutput = new ExerciseOutput(outputContent, exercise);

        // Assert
        exerciseOutput.OutputContent.Should().Be(outputContent);
    }

    [Fact]
    public void Constructor_Should_AcceptMultilineOutput()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var outputContent = "5\n10\n15\n20\n25";

        // Act
        var exerciseOutput = new ExerciseOutput(outputContent, exercise);

        // Assert
        exerciseOutput.OutputContent.Should().Be(outputContent);
    }

    [Fact]
    public void Constructor_Should_AcceptComplexOutput()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var outputContent = "Result: 150\nSum: 75\nAverage: 15.0";

        // Act
        var exerciseOutput = new ExerciseOutput(outputContent, exercise);

        // Assert
        exerciseOutput.OutputContent.Should().Be(outputContent);
    }

    [Fact]
    public void Constructor_Should_AcceptLargeOutput()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var outputContent = new string('B', 10000);

        // Act
        var exerciseOutput = new ExerciseOutput(outputContent, exercise);

        // Assert
        exerciseOutput.OutputContent.Should().HaveLength(10000);
    }

    [Fact]
    public void ExerciseOutput_Should_SupportMultipleOutputsForSameExercise()
    {
        // Arrange
        var exercise = CreateTestExercise();

        // Act
        var output1 = new ExerciseOutput("output 1", exercise);
        var output2 = new ExerciseOutput("output 2", exercise);

        // Assert
        output1.ExerciseId.Should().Be(exercise.Id);
        output2.ExerciseId.Should().Be(exercise.Id);
        output1.OutputContent.Should().NotBe(output2.OutputContent);
    }

    [Fact]
    public void Constructor_Should_MaintainExerciseReference()
    {
        // Arrange
        var exercise = CreateTestExercise();

        // Act
        var exerciseOutput = new ExerciseOutput("output", exercise);

        // Assert
        exerciseOutput.Exercise.Should().NotBeNull();
        exerciseOutput.Exercise.Should().BeSameAs(exercise);
    }

    [Fact]
    public void Constructor_Should_AcceptOutputWithSpecialCharacters()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var outputContent = "Output: @#$%&*()[]{}!?\nNew line\tTab";

        // Act
        var exerciseOutput = new ExerciseOutput(outputContent, exercise);

        // Assert
        exerciseOutput.OutputContent.Should().Be(outputContent);
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
