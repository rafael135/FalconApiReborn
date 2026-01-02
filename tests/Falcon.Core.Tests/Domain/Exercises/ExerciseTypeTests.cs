using Falcon.Core.Domain.Exercises;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Exercises;

public class ExerciseTypeTests
{
    [Fact]
    public void Constructor_Should_CreateExerciseType_WithValidParameters()
    {
        // Arrange
        var id = 1;
        var label = "Algorithm";

        // Act
        var exerciseType = new ExerciseType(id, label);

        // Assert
        exerciseType.Should().NotBeNull();
        exerciseType.Id.Should().Be(id);
        exerciseType.Label.Should().Be(label);
        exerciseType.Exercises.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenLabelIsInvalid(string? invalidLabel)
    {
        // Act
        Action act = () => new ExerciseType(1, invalidLabel!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Rótulo*obrigatório*");
    }

    [Fact]
    public void Constructor_Should_AcceptDifferentIds()
    {
        // Act
        var type1 = new ExerciseType(1, "Type 1");
        var type2 = new ExerciseType(2, "Type 2");
        var type3 = new ExerciseType(999, "Type 999");

        // Assert
        type1.Id.Should().Be(1);
        type2.Id.Should().Be(2);
        type3.Id.Should().Be(999);
    }

    [Fact]
    public void Constructor_Should_AcceptVariousLabels()
    {
        // Act
        var algorithmType = new ExerciseType(1, "Algorithm");
        var dataStructureType = new ExerciseType(2, "Data Structure");
        var mathType = new ExerciseType(3, "Mathematics");

        // Assert
        algorithmType.Label.Should().Be("Algorithm");
        dataStructureType.Label.Should().Be("Data Structure");
        mathType.Label.Should().Be("Mathematics");
    }

    [Fact]
    public void Exercises_Should_BeReadOnlyCollection()
    {
        // Arrange
        var exerciseType = new ExerciseType(1, "Algorithm");

        // Act
        var exercises = exerciseType.Exercises;

        // Assert
        exercises.Should().BeAssignableTo<IReadOnlyCollection<Exercise>>();
    }

    [Fact]
    public void Constructor_Should_InitializeEmptyExercisesCollection()
    {
        // Arrange & Act
        var exerciseType = new ExerciseType(1, "Algorithm");

        // Assert
        exerciseType.Exercises.Should().NotBeNull();
        exerciseType.Exercises.Should().BeEmpty();
        exerciseType.Exercises.Should().HaveCount(0);
    }

    [Fact]
    public void Constructor_Should_AcceptLongLabels()
    {
        // Arrange
        var longLabel = "Very Long Exercise Type Label With Many Words And Characters To Test Maximum Length Handling";

        // Act
        var exerciseType = new ExerciseType(1, longLabel);

        // Assert
        exerciseType.Label.Should().Be(longLabel);
    }

    [Fact]
    public void Constructor_Should_AcceptSpecialCharactersInLabel()
    {
        // Arrange
        var label = "C++ & C# Programming";

        // Act
        var exerciseType = new ExerciseType(1, label);

        // Assert
        exerciseType.Label.Should().Be(label);
    }

    [Fact]
    public void Constructor_Should_AcceptNegativeId()
    {
        // Note: In practice, IDs are usually positive, but no validation exists
        // Arrange & Act
        var exerciseType = new ExerciseType(-1, "Test");

        // Assert
        exerciseType.Id.Should().Be(-1);
    }

    [Fact]
    public void Constructor_Should_AcceptZeroId()
    {
        // Arrange & Act
        var exerciseType = new ExerciseType(0, "Test");

        // Assert
        exerciseType.Id.Should().Be(0);
    }
}
