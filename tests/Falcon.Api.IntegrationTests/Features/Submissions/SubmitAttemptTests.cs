using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Falcon.Api.IntegrationTests.Features.Submissions;

public class SubmitAttemptTests : TestBase
{
    public SubmitAttemptTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_ReturnSuccess_When_ValidSubmission()
    {
        // Arrange
        var (student, token) = await CreateStudentAsync();
        var group = await CreateGroupAsync(student, "Test Group");
        var exercise = await CreateExerciseAsync();
        
        // Configure exercise with Judge UUID (required for submission)
        var exerciseFromDb = await DbContext.Exercises.FindAsync(exercise.Id);
        exerciseFromDb!.SetJudgeUuid("test-judge-uuid");
        
        // Create a competition and add exercise
        var competition = await CreateCompetitionAsync("Test Competition");
        competition.Start();
        competition.AddExercise(exercise);
        
        // Register group in competition (Group first, Competition second)
        var groupInComp = new Falcon.Core.Domain.Competitions.GroupInCompetition(group, competition);
        DbContext.GroupsInCompetitions.Add(groupInComp);
        
        // Need to save to persist ExerciseInCompetition and GroupInCompetition
        await DbContext.SaveChangesAsync();
        
        HttpClient.SetBearerToken(token);

        var command = new
        {
            CompetitionId = competition.Id,
            ExerciseId = exercise.Id,
            Code = "print('Hello World')",
            Language = 1
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Submission", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Should_ReturnUnauthorized_When_NotAuthenticated()
    {
        // Arrange
        HttpClient.ClearAuthorization();

        var command = new
        {
            CompetitionId = Guid.NewGuid(),
            ExerciseId = Guid.NewGuid(),
            Code = "print('Hello World')",
            Language = 1
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("/api/Submission", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
