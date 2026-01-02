using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Files;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Files;

public class AttachedFileTests
{
    [Fact]
    public void Constructor_Should_CreateAttachedFile_WithValidParameters()
    {
        // Arrange
        var name = "test-file.pdf";
        var type = "application/pdf";
        var size = 1024L;
        var filePath = "/uploads/test-file.pdf";

        // Act
        var attachedFile = new AttachedFile(name, type, size, filePath);

        // Assert
        attachedFile.Should().NotBeNull();
        attachedFile.Name.Should().Be(name);
        attachedFile.Type.Should().Be(type);
        attachedFile.Size.Should().Be(size);
        attachedFile.FilePath.Should().Be(filePath);
        attachedFile.Exercises.Should().BeEmpty();
        attachedFile.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenNameIsInvalid(string? invalidName)
    {
        // Act
        Action act = () => new AttachedFile(invalidName!, "type", 1024L, "/path");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Nome*arquivo*obrigatório*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenTypeIsInvalid(string? invalidType)
    {
        // Act
        Action act = () => new AttachedFile("name", invalidType!, 1024L, "/path");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tipo*arquivo*obrigatório*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-1000)]
    public void Constructor_Should_ThrowArgumentException_WhenSizeIsInvalid(long invalidSize)
    {
        // Act
        Action act = () => new AttachedFile("name", "type", invalidSize, "/path");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Tamanho*arquivo*maior que 0*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenFilePathIsInvalid(string? invalidPath)
    {
        // Act
        Action act = () => new AttachedFile("name", "type", 1024L, invalidPath!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Caminho*arquivo*obrigatório*");
    }

    [Fact]
    public void AttachToExercise_Should_AttachExercise()
    {
        // Arrange
        var attachedFile = CreateTestFile();
        var exercise = CreateTestExercise();

        // Act
        attachedFile.AttachToExercise(exercise);

        // Assert
        attachedFile.Exercises.Should().Contain(exercise);
        attachedFile.Exercises.Should().HaveCount(1);
    }

    [Fact]
    public void AttachToExercise_Should_ThrowArgumentNullException_WhenExerciseIsNull()
    {
        // Arrange
        var attachedFile = CreateTestFile();

        // Act
        Action act = () => attachedFile.AttachToExercise(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("exercise");
    }

    [Fact]
    public void AttachToExercise_Should_NotAddDuplicateExercise()
    {
        // Arrange
        var attachedFile = CreateTestFile();
        var exercise = CreateTestExercise();

        // Act
        attachedFile.AttachToExercise(exercise);
        attachedFile.AttachToExercise(exercise);
        attachedFile.AttachToExercise(exercise);

        // Assert
        attachedFile.Exercises.Should().HaveCount(1);
        attachedFile.Exercises.Should().Contain(exercise);
    }

    [Fact]
    public void AttachToExercise_Should_NotThrowException()
    {
        // Arrange
        var attachedFile = CreateTestFile();
        var exercise = CreateTestExercise();

        // Act
        Action act = () => attachedFile.AttachToExercise(exercise);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void Constructor_Should_AcceptVariousFileTypes()
    {
        // Arrange & Act
        var pdfFile = new AttachedFile("doc.pdf", "application/pdf", 1024L, "/path");
        var imageFile = new AttachedFile("image.jpg", "image/jpeg", 2048L, "/path");
        var textFile = new AttachedFile("text.txt", "text/plain", 512L, "/path");

        // Assert
        pdfFile.Type.Should().Be("application/pdf");
        imageFile.Type.Should().Be("image/jpeg");
        textFile.Type.Should().Be("text/plain");
    }

    [Fact]
    public void Constructor_Should_AcceptLargeFileSize()
    {
        // Arrange
        var largeSize = 1024L * 1024L * 100L; // 100 MB

        // Act
        var attachedFile = new AttachedFile("large-file.zip", "application/zip", largeSize, "/path");

        // Assert
        attachedFile.Size.Should().Be(largeSize);
    }

    [Fact]
    public void Constructor_Should_AcceptSmallFileSize()
    {
        // Arrange
        var smallSize = 1L; // 1 byte

        // Act
        var attachedFile = new AttachedFile("tiny-file.txt", "text/plain", smallSize, "/path");

        // Assert
        attachedFile.Size.Should().Be(smallSize);
    }

    [Fact]
    public void Constructor_Should_AcceptDifferentPathFormats()
    {
        // Arrange & Act
        var windowsPath = new AttachedFile("file1.txt", "text/plain", 100L, "C:\\uploads\\file1.txt");
        var unixPath = new AttachedFile("file2.txt", "text/plain", 100L, "/var/uploads/file2.txt");
        var relativePath = new AttachedFile("file3.txt", "text/plain", 100L, "uploads/file3.txt");

        // Assert
        windowsPath.FilePath.Should().Be("C:\\uploads\\file1.txt");
        unixPath.FilePath.Should().Be("/var/uploads/file2.txt");
        relativePath.FilePath.Should().Be("uploads/file3.txt");
    }

    [Fact]
    public void Exercises_Should_BeReadOnlyCollection()
    {
        // Arrange
        var attachedFile = CreateTestFile();
        var exercise = CreateTestExercise();
        attachedFile.AttachToExercise(exercise);

        // Act
        var exercises = attachedFile.Exercises;

        // Assert
        exercises.Should().BeAssignableTo<IReadOnlyCollection<Exercise>>();
    }

    [Fact]
    public void CreatedAt_Should_NotChangeAfterAttachingExercise()
    {
        // Arrange
        var attachedFile = CreateTestFile();
        var originalCreatedAt = attachedFile.CreatedAt;
        var exercise = CreateTestExercise();

        // Act
        Thread.Sleep(10);
        attachedFile.AttachToExercise(exercise);

        // Assert
        attachedFile.CreatedAt.Should().Be(originalCreatedAt);
    }

    // Helper methods
    private static AttachedFile CreateTestFile()
    {
        return new AttachedFile(
            "test-file.pdf",
            "application/pdf",
            1024L,
            "/uploads/test-file.pdf");
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
