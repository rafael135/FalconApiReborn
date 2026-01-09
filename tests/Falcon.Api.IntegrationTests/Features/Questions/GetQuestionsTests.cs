using System.Net;
using System.Text.Json;
using Falcon.Api.IntegrationTests.Helpers;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Enums;
using Falcon.Core.Domain.Users;
using FluentAssertions;

namespace Falcon.Api.IntegrationTests.Features.Questions;

public class GetQuestionsTests : TestBase
{
    public GetQuestionsTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    private async Task<Question> CreateQuestionAsync(
        Competition competition, 
        User user, 
        Exercise? exercise = null, 
        QuestionType type = QuestionType.General)
    {
        var question = new Question(
            competition, 
            user, 
            $"Test question content {Guid.NewGuid()}", 
            type, 
            exercise
        );
        await DbContext.Questions.AddAsync(question);
        await DbContext.SaveChangesAsync();
        return question;
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_GettingQuestionsForCompetition()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var competition = await CreateCompetitionAsync();
        
        await CreateQuestionAsync(competition, student);
        await CreateQuestionAsync(competition, student);
        await CreateQuestionAsync(competition, student);
        
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/Question?competitionId={competition.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("questions").GetArrayLength().Should().Be(3);
        jsonDoc.RootElement.GetProperty("totalCount").GetInt32().Should().Be(3);
    }

    [Fact]
    public async Task Should_ReturnEmpty_When_CompetitionHasNoQuestions()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var competition = await CreateCompetitionAsync();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/Question?competitionId={competition.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("questions").GetArrayLength().Should().Be(0);
        jsonDoc.RootElement.GetProperty("totalCount").GetInt32().Should().Be(0);
    }

    [Fact]
    public async Task Should_FilterByExercise_When_ExerciseIdProvided()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var competition = await CreateCompetitionAsync();
        var exercise1 = await CreateExerciseAsync("Exercise 1");
        var exercise2 = await CreateExerciseAsync("Exercise 2");
        
        await CreateQuestionAsync(competition, student, exercise1);
        await CreateQuestionAsync(competition, student, exercise1);
        await CreateQuestionAsync(competition, student, exercise2);
        
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/Question?competitionId={competition.Id}&exerciseId={exercise1.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("questions").GetArrayLength().Should().Be(2);
    }

    [Fact]
    public async Task Should_FilterByQuestionType_When_TypeProvided()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var competition = await CreateCompetitionAsync();
        
        await CreateQuestionAsync(competition, student, type: QuestionType.General);
        await CreateQuestionAsync(competition, student, type: QuestionType.General);
        await CreateQuestionAsync(competition, student, type: QuestionType.Technical);
        
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/Question?competitionId={competition.Id}&questionType=1"); // General = 1

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("questions").GetArrayLength().Should().Be(2);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        var competition = await CreateCompetitionAsync();
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.GetAsync($"/api/Question?competitionId={competition.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_Paginate_When_SkipAndTakeProvided()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var competition = await CreateCompetitionAsync();
        
        // Create 15 questions
        for (int i = 0; i < 15; i++)
        {
            await CreateQuestionAsync(competition, student);
        }
        
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync(
            $"/api/Question?competitionId={competition.Id}&skip=5&take=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("questions").GetArrayLength().Should().Be(5);
        jsonDoc.RootElement.GetProperty("totalCount").GetInt32().Should().Be(15);
        jsonDoc.RootElement.GetProperty("skip").GetInt32().Should().Be(5);
        jsonDoc.RootElement.GetProperty("take").GetInt32().Should().Be(5);
    }
}
