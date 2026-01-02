using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Exceptions;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Exercises;

public class ExerciseTests
{
    [Fact]
    public void Constructor_Should_CreateExercise_WithValidParameters()
    {
        // Arrange
        var title = "Fibonacci Sequence";
        var description = "Calculate the nth Fibonacci number";
        var exerciseTypeId = 1;
        var estimatedTime = TimeSpan.FromMinutes(30);

        // Act
        var exercise = new Exercise(title, description, exerciseTypeId, estimatedTime);

        // Assert
        exercise.Should().NotBeNull();
        exercise.Title.Should().Be(title);
        exercise.Description.Should().Be(description);
        exercise.ExerciseTypeId.Should().Be(exerciseTypeId);
        exercise.EstimatedTime.Should().Be(estimatedTime);
        exercise.Inputs.Should().BeEmpty();
        exercise.Outputs.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenTitleIsInvalid(string? invalidTitle)
    {
        // Act
        Action act = () => new Exercise(invalidTitle!, "Description", 1, TimeSpan.FromMinutes(30));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Título*obrigatório*");
    }

    [Fact]
    public void AddTestCase_Should_AddInputAndOutputToExercise()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var input = "5";
        var expectedOutput = "120";

        // Act
        exercise.AddTestCase(input, expectedOutput);

        // Assert
        exercise.Inputs.Should().ContainSingle();
        exercise.Outputs.Should().ContainSingle();
    }

    [Fact]
    public void ClearTestCases_Should_RemoveAllInputsAndOutputs()
    {
        // Arrange
        var exercise = CreateTestExercise();
        exercise.AddTestCase("1", "1");
        exercise.AddTestCase("2", "2");

        // Act
        exercise.ClearTestCases();

        // Assert
        exercise.Inputs.Should().BeEmpty();
        exercise.Outputs.Should().BeEmpty();
    }

    [Fact]
    public void UpdateDetails_Should_ModifyExerciseInformation()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var newTitle = "Updated Title";
        var newDescription = "Updated Description";
        var newTypeId = 2;

        // Act
        exercise.UpdateDetails(newTitle, newDescription, newTypeId);

        // Assert
        exercise.Title.Should().Be(newTitle);
        exercise.Description.Should().Be(newDescription);
        exercise.ExerciseTypeId.Should().Be(newTypeId);
    }

    [Fact]
    public void SetJudgeUuid_Should_UpdateJudgeUuid()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var judgeUuid = "abc-123-def-456";

        // Act
        exercise.SetJudgeUuid(judgeUuid);

        // Assert
        exercise.JudgeUuid.Should().Be(judgeUuid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetJudgeUuid_Should_ThrowBusinessRuleException_WhenInvalid(string? invalidUuid)
    {
        // Arrange
        var exercise = CreateTestExercise();

        // Act
        Action act = () => exercise.SetJudgeUuid(invalidUuid!);

        // Assert
        act.Should().Throw<BusinessRuleException>()
            .WithMessage("*UUID*obrigatório*");
    }

    // Helper methods
    private static Exercise CreateTestExercise()
    {
        return new Exercise(
            "Test Exercise",
            "Test Description",
            1,
            TimeSpan.FromMinutes(30)
        );
    }
}
