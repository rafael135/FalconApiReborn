using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Users;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Exercises;

public class GroupExerciseAttemptTests
{
    [Fact]
    public void Constructor_Should_CreateAttempt_WithValidParameters()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var group = CreateTestGroup();
        var competition = CreateTestCompetition();
        var code = "console.log('Hello World');";
        var language = LanguageType.JavaScript;

        // Act
        var attempt = new GroupExerciseAttempt(exercise, group, competition, code, language);

        // Assert
        attempt.Should().NotBeNull();
        attempt.Exercise.Should().Be(exercise);
        attempt.ExerciseId.Should().Be(exercise.Id);
        attempt.Group.Should().Be(group);
        attempt.GroupId.Should().Be(group.Id);
        attempt.Competition.Should().Be(competition);
        attempt.CompetitionId.Should().Be(competition.Id);
        attempt.Code.Should().Be(code);
        attempt.Language.Should().Be(language);
        attempt.Time.Should().Be(TimeSpan.Zero);
        attempt.Accepted.Should().BeFalse();
        attempt.JudgeResponse.Should().Be(JudgeSubmissionResponse.Pending);
        attempt.SubmissionTime.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenExerciseIsNull()
    {
        // Arrange
        var group = CreateTestGroup();
        var competition = CreateTestCompetition();

        // Act
        Action act = () => new GroupExerciseAttempt(null!, group, competition, "code", LanguageType.CSharp);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("exercise");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenGroupIsNull()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var competition = CreateTestCompetition();

        // Act
        Action act = () => new GroupExerciseAttempt(exercise, null!, competition, "code", LanguageType.CSharp);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("group");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenCompetitionIsNull()
    {
        // Arrange
        var exercise = CreateTestExercise();
        var group = CreateTestGroup();

        // Act
        Action act = () => new GroupExerciseAttempt(exercise, group, null!, "code", LanguageType.CSharp);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("competition");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenCodeIsInvalid(string? invalidCode)
    {
        // Arrange
        var exercise = CreateTestExercise();
        var group = CreateTestGroup();
        var competition = CreateTestCompetition();

        // Act
        Action act = () => new GroupExerciseAttempt(exercise, group, competition, invalidCode!, LanguageType.CSharp);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*código*vazio*");
    }

    [Theory]
    [InlineData(LanguageType.CSharp)]
    [InlineData(LanguageType.Java)]
    [InlineData(LanguageType.Python)]
    [InlineData(LanguageType.JavaScript)]
    [InlineData(LanguageType.Cpp)]
    [InlineData(LanguageType.C)]
    public void Constructor_Should_AcceptAllLanguageTypes(LanguageType language)
    {
        // Arrange
        var exercise = CreateTestExercise();
        var group = CreateTestGroup();
        var competition = CreateTestCompetition();

        // Act
        var attempt = new GroupExerciseAttempt(exercise, group, competition, "code", language);

        // Assert
        attempt.Language.Should().Be(language);
    }

    [Fact]
    public void SetJudgeResponse_Should_SetAcceptedToTrue_WhenResponseIsAccepted()
    {
        // Arrange
        var attempt = CreateTestAttempt();
        var executionTime = TimeSpan.FromMilliseconds(250);

        // Act
        attempt.SetJudgeResponse(JudgeSubmissionResponse.Accepted, executionTime);

        // Assert
        attempt.JudgeResponse.Should().Be(JudgeSubmissionResponse.Accepted);
        attempt.Accepted.Should().BeTrue();
        attempt.Time.Should().Be(executionTime);
    }

    [Fact]
    public void SetJudgeResponse_Should_KeepAcceptedFalse_WhenResponseIsWrongAnswer()
    {
        // Arrange
        var attempt = CreateTestAttempt();
        var executionTime = TimeSpan.FromMilliseconds(100);

        // Act
        attempt.SetJudgeResponse(JudgeSubmissionResponse.WrongAnswer, executionTime);

        // Assert
        attempt.JudgeResponse.Should().Be(JudgeSubmissionResponse.WrongAnswer);
        attempt.Accepted.Should().BeFalse();
        attempt.Time.Should().Be(executionTime);
    }

    [Fact]
    public void SetJudgeResponse_Should_KeepAcceptedFalse_WhenResponseIsCompilationError()
    {
        // Arrange
        var attempt = CreateTestAttempt();
        var executionTime = TimeSpan.Zero;

        // Act
        attempt.SetJudgeResponse(JudgeSubmissionResponse.CompilationError, executionTime);

        // Assert
        attempt.JudgeResponse.Should().Be(JudgeSubmissionResponse.CompilationError);
        attempt.Accepted.Should().BeFalse();
        attempt.Time.Should().Be(executionTime);
    }

    [Fact]
    public void SetJudgeResponse_Should_KeepAcceptedFalse_WhenResponseIsTimeLimitExceeded()
    {
        // Arrange
        var attempt = CreateTestAttempt();
        var executionTime = TimeSpan.FromSeconds(5);

        // Act
        attempt.SetJudgeResponse(JudgeSubmissionResponse.TimeLimitExceeded, executionTime);

        // Assert
        attempt.JudgeResponse.Should().Be(JudgeSubmissionResponse.TimeLimitExceeded);
        attempt.Accepted.Should().BeFalse();
        attempt.Time.Should().Be(executionTime);
    }

    [Fact]
    public void SetJudgeResponse_Should_KeepAcceptedFalse_WhenResponseIsRuntimeError()
    {
        // Arrange
        var attempt = CreateTestAttempt();
        var executionTime = TimeSpan.FromMilliseconds(50);

        // Act
        attempt.SetJudgeResponse(JudgeSubmissionResponse.RuntimeError, executionTime);

        // Assert
        attempt.JudgeResponse.Should().Be(JudgeSubmissionResponse.RuntimeError);
        attempt.Accepted.Should().BeFalse();
        attempt.Time.Should().Be(executionTime);
    }

    [Fact]
    public void SetJudgeResponse_Should_RecordExecutionTime()
    {
        // Arrange
        var attempt = CreateTestAttempt();
        var executionTime = TimeSpan.FromMilliseconds(123);

        // Act
        attempt.SetJudgeResponse(JudgeSubmissionResponse.Accepted, executionTime);

        // Assert
        attempt.Time.Should().Be(executionTime);
    }

    [Theory]
    [InlineData(JudgeSubmissionResponse.Pending)]
    [InlineData(JudgeSubmissionResponse.Accepted)]
    [InlineData(JudgeSubmissionResponse.WrongAnswer)]
    [InlineData(JudgeSubmissionResponse.CompilationError)]
    [InlineData(JudgeSubmissionResponse.RuntimeError)]
    [InlineData(JudgeSubmissionResponse.TimeLimitExceeded)]
    [InlineData(JudgeSubmissionResponse.MemoryLimitExceeded)]
    [InlineData(JudgeSubmissionResponse.InternalError)]
    public void SetJudgeResponse_Should_HandleAllJudgeResponseTypes(JudgeSubmissionResponse response)
    {
        // Arrange
        var attempt = CreateTestAttempt();
        var executionTime = TimeSpan.FromMilliseconds(100);

        // Act
        attempt.SetJudgeResponse(response, executionTime);

        // Assert
        attempt.JudgeResponse.Should().Be(response);
        attempt.Accepted.Should().Be(response == JudgeSubmissionResponse.Accepted);
    }

    // Helper methods
    private static GroupExerciseAttempt CreateTestAttempt()
    {
        return new GroupExerciseAttempt(
            CreateTestExercise(),
            CreateTestGroup(),
            CreateTestCompetition(),
            "console.log('test');",
            LanguageType.JavaScript);
    }

    private static Exercise CreateTestExercise()
    {
        return new Exercise(
            "Test Exercise",
            "Description",
            1,
            TimeSpan.FromMinutes(30));
    }

    private static Group CreateTestGroup()
    {
        var leader = new User("Leader", "leader@test.com", "12345");
        return new Group("Test Group", leader);
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
}
