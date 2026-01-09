using System.Net;
using System.Text.Json;
using Falcon.Api.IntegrationTests.Helpers;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Shared.Enums;
using FluentAssertions;

namespace Falcon.Api.IntegrationTests.Features.Questions;

public class GetQuestionTests : TestBase
{
    public GetQuestionTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_GettingQuestionById()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var competition = await CreateCompetitionAsync();
        
        var question = new Question(
            competition, 
            student, 
            "Test question content", 
            QuestionType.General
        );
        await DbContext.Questions.AddAsync(question);
        await DbContext.SaveChangesAsync();
        
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/Question/{question.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        jsonDoc.RootElement.GetProperty("id").GetGuid().Should().Be(question.Id);
        jsonDoc.RootElement.GetProperty("content").GetString().Should().Be("Test question content");
    }

    [Fact]
    public async Task Should_ReturnNotFound_When_QuestionDoesNotExist()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var nonExistentId = Guid.NewGuid();
        HttpClient.SetBearerToken(token);

        // Act
        var response = await HttpClient.GetAsync($"/api/Question/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        var (student, _) = await CreateStudentAsync();
        var competition = await CreateCompetitionAsync();
        
        var question = new Question(competition, student, "Test", QuestionType.General);
        await DbContext.Questions.AddAsync(question);
        await DbContext.SaveChangesAsync();
        
        HttpClient.ClearAuthorization();

        // Act
        var response = await HttpClient.GetAsync($"/api/Question/{question.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Should_IncludeAnswer_When_QuestionIsAnswered()
    {
        // Arrange
        var (student, studentToken) = await CreateStudentAsync();
        var (teacher, _) = await CreateTeacherAsync();
        var competition = await CreateCompetitionAsync();
        
        var question = new Question(competition, student, "Test question", QuestionType.General);
        await DbContext.Questions.AddAsync(question);
        await DbContext.SaveChangesAsync();
        
        var answer = new Answer(teacher, "This is the answer");
        await DbContext.Answers.AddAsync(answer);
        question.SetAnswer(answer);
        await DbContext.SaveChangesAsync();
        
        HttpClient.SetBearerToken(studentToken);

        // Act
        var response = await HttpClient.GetAsync($"/api/Question/{question.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(json);
        
        var answerElement = jsonDoc.RootElement.GetProperty("answer");
        answerElement.ValueKind.Should().NotBe(JsonValueKind.Null);
        answerElement.GetProperty("content").GetString().Should().Be("This is the answer");
        answerElement.GetProperty("userId").GetString().Should().Be(teacher.Id);
    }
}
