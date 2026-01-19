using Falcon.Api.IntegrationTests.Helpers;
using FluentAssertions;
using Quartz;
using Quartz.Impl.Matchers;
using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Falcon.Api.IntegrationTests.Features.Competitions;

public class CompetitionSchedulingTests : TestBase
{
    public CompetitionSchedulingTests(CustomWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Should_Schedule_Jobs_When_TemplatePromoted()
    {
        // Arrange
        var (teacher, token) = await CreateTeacherAsync();
        HttpClient.SetBearerToken(token);

        var template = await CreateTemplateAsync(
            "Test Competition",
            "Test Description",
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(2)
        );

        var command = new
        {
            TemplateId = template.Id,
            MaxMembers = 3,
            MaxExercises = 3,
            MaxSubmissionSize = 1024,
            Duration = TimeSpan.FromHours(2),
            StopRanking = TimeSpan.FromMinutes(30),
            BlockSubmissions = TimeSpan.FromMinutes(10),
            Penalty = TimeSpan.FromMinutes(5)
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"/api/Competition/{template.Id}/promote", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Get scheduler and assert jobs are scheduled for the competition
        var schedulerFactory = Scope.ServiceProvider.GetRequiredService<ISchedulerFactory>();
        var scheduler = await schedulerFactory.GetScheduler();

        var groupMatcher = GroupMatcher<JobKey>.GroupEquals($"Competition-{template.Id}");
        var jobKeys = await scheduler.GetJobKeys(groupMatcher);

        jobKeys.Count.Should().BeGreaterThan(0);
    }
}
