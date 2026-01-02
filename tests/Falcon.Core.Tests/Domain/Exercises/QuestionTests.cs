using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Users;
using FluentAssertions;
using Xunit;

namespace Falcon.Core.Tests.Domain.Exercises;

public class QuestionTests
{
    [Fact]
    public void Constructor_Should_CreateQuestion_WithValidParameters()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var user = CreateTestUser();
        var content = "Como faço para resolver este exercício?";
        var questionType = QuestionType.General;

        // Act
        var question = new Question(competition, user, content, questionType);

        // Assert
        question.Should().NotBeNull();
        question.Competition.Should().Be(competition);
        question.CompetitionId.Should().Be(competition.Id);
        question.User.Should().Be(user);
        question.UserId.Should().Be(user.Id);
        question.Content.Should().Be(content);
        question.QuestionType.Should().Be(questionType);
        question.Exercise.Should().BeNull();
        question.ExerciseId.Should().BeNull();
        question.Answer.Should().BeNull();
        question.AnswerId.Should().BeNull();
        question.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_Should_CreateQuestion_WithExercise()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var user = CreateTestUser();
        var exercise = CreateTestExercise();
        var content = "Qual é o formato esperado da saída?";
        var questionType = QuestionType.Technical;

        // Act
        var question = new Question(competition, user, content, questionType, exercise);

        // Assert
        question.Should().NotBeNull();
        question.Exercise.Should().Be(exercise);
        question.ExerciseId.Should().Be(exercise.Id);
        question.Competition.Should().Be(competition);
        question.User.Should().Be(user);
        question.Content.Should().Be(content);
        question.QuestionType.Should().Be(questionType);
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenCompetitionIsNull()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        Action act = () => new Question(null!, user, "content", QuestionType.General);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("competition");
    }

    [Fact]
    public void Constructor_Should_ThrowArgumentNullException_WhenUserIsNull()
    {
        // Arrange
        var competition = CreateTestCompetition();

        // Act
        Action act = () => new Question(competition, null!, "content", QuestionType.General);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("user");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowArgumentException_WhenContentIsInvalid(string? invalidContent)
    {
        // Arrange
        var competition = CreateTestCompetition();
        var user = CreateTestUser();

        // Act
        Action act = () => new Question(competition, user, invalidContent!, QuestionType.General);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*conteúdo*questão*vazio*");
    }

    [Theory]
    [InlineData(QuestionType.General)]
    [InlineData(QuestionType.Technical)]
    [InlineData(QuestionType.Clarification)]
    [InlineData(QuestionType.BugReport)]
    public void Constructor_Should_AcceptAllQuestionTypes(QuestionType questionType)
    {
        // Arrange
        var competition = CreateTestCompetition();
        var user = CreateTestUser();
        var content = "Test question content";

        // Act
        var question = new Question(competition, user, content, questionType);

        // Assert
        question.QuestionType.Should().Be(questionType);
    }

    [Fact]
    public void SetAnswer_Should_SetAnswerAndAnswerId()
    {
        // Arrange
        var question = CreateTestQuestion();
        var answerUser = CreateTestUser();
        var answer = new Answer(answerUser, "Esta é a resposta para sua pergunta.");

        // Act
        question.SetAnswer(answer);

        // Assert
        question.Answer.Should().Be(answer);
        question.AnswerId.Should().Be(answer.Id);
    }

    [Fact]
    public void SetAnswer_Should_ThrowArgumentNullException_WhenAnswerIsNull()
    {
        // Arrange
        var question = CreateTestQuestion();

        // Act
        Action act = () => question.SetAnswer(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("answer");
    }

    [Fact]
    public void Question_Should_SupportGeneralCompetitionQuestion()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var user = CreateTestUser();
        var content = "Quando a competição começará?";

        // Act
        var question = new Question(competition, user, content, QuestionType.General);

        // Assert
        question.Exercise.Should().BeNull();
        question.ExerciseId.Should().BeNull();
        question.QuestionType.Should().Be(QuestionType.General);
    }

    [Fact]
    public void Question_Should_SupportExerciseSpecificQuestion()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var user = CreateTestUser();
        var exercise = CreateTestExercise();
        var content = "Este exercício está com problema no caso de teste 3?";

        // Act
        var question = new Question(competition, user, content, QuestionType.BugReport, exercise);

        // Assert
        question.Exercise.Should().NotBeNull();
        question.ExerciseId.Should().Be(exercise.Id);
        question.QuestionType.Should().Be(QuestionType.BugReport);
    }

    [Fact]
    public void Question_Should_AllowMultipleQuestionsFromSameUser()
    {
        // Arrange
        var competition = CreateTestCompetition();
        var user = CreateTestUser();

        // Act
        var question1 = new Question(competition, user, "Primeira pergunta", QuestionType.General);
        var question2 = new Question(competition, user, "Segunda pergunta", QuestionType.Technical);

        // Assert
        question1.UserId.Should().Be(user.Id);
        question2.UserId.Should().Be(user.Id);
        question1.Content.Should().NotBe(question2.Content);
    }

    // Helper methods
    private static Question CreateTestQuestion()
    {
        return new Question(
            CreateTestCompetition(),
            CreateTestUser(),
            "Test question?",
            QuestionType.General);
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

    private static User CreateTestUser()
    {
        return new User("Test User", "test@example.com", "12345");
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
