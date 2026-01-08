using Falcon.Api.IntegrationTests.Helpers;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Users;
using Falcon.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Falcon.Api.IntegrationTests;

public abstract class TestBase : IClassFixture<CustomWebApplicationFactory>, IDisposable
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient HttpClient;
    protected readonly FalconDbContext DbContext;
    protected readonly UserManager<User> UserManager;
    protected readonly SignInManager<User> SignInManager;
    protected readonly RoleManager<IdentityRole> RoleManager;
    protected readonly IServiceScope Scope;

    protected TestBase(CustomWebApplicationFactory factory)
    {
        Factory = factory;
        Scope = factory.Services.CreateScope();
        HttpClient = factory.CreateClient();
        DbContext = Scope.ServiceProvider.GetRequiredService<FalconDbContext>();
        UserManager = Scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        SignInManager = Scope.ServiceProvider.GetRequiredService<SignInManager<User>>();
        RoleManager = Scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Ensure database is created
        DbContext.Database.EnsureCreated();

        // Ensure roles exist
        EnsureRolesExistAsync().GetAwaiter().GetResult();
    }

    private async Task EnsureRolesExistAsync()
    {
        string[] roleNames = { "Admin", "Teacher", "Student" };
        foreach (var roleName in roleNames)
        {
            if (!await RoleManager.RoleExistsAsync(roleName))
            {
                await RoleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    protected async Task<(User User, string Token)> CreateStudentAsync(
        string email = "student@example.com",
        string name = "Test Student",
        string ra = null!,
        string password = "password123")
    {
        ra ??= Guid.NewGuid().ToString().Substring(0, 6);
        return await AuthenticationHelper.CreateAuthenticatedStudentAsync(UserManager, email, name, ra, password);
    }

    protected async Task<(User User, string Token)> CreateTeacherAsync(
        string email = "teacher@example.com",
        string name = "Test Teacher",
        string ra = null!,
        string password = "password123")
    {
        ra ??= Guid.NewGuid().ToString().Substring(0, 6);
        return await AuthenticationHelper.CreateAuthenticatedTeacherAsync(UserManager, email, name, ra, password);
    }

    protected async Task<(User User, string Token)> CreateAdminAsync(
        string email = "admin@example.com",
        string name = "Test Admin",
        string ra = null!,
        string password = "password123")
    {
        ra ??= Guid.NewGuid().ToString().Substring(0, 6);
        return await AuthenticationHelper.CreateAuthenticatedAdminAsync(UserManager, email, name, ra, password);
    }

    protected async Task<Group> CreateGroupAsync(User leader, string name = "Test Group")
    {
        var group = new Group(name, leader);
        DbContext.Groups.Add(group);
        await DbContext.SaveChangesAsync();
        leader.AssignGroup(group);
        await DbContext.SaveChangesAsync();
        return group;
    }

    protected async Task<Competition> CreateTemplateAsync(
        string name = "Test Competition",
        string description = "Test Description",
        DateTime? startInscriptions = null,
        DateTime? endInscriptions = null,
        DateTime? startTime = null)
    {
        var now = DateTime.UtcNow;
        startInscriptions ??= now.AddDays(-1);
        endInscriptions ??= now.AddDays(1);
        startTime ??= now.AddDays(2);

        var template = Competition.CreateTemplate(name, description, startInscriptions.Value, endInscriptions.Value, startTime.Value);
        DbContext.Competitions.Add(template);
        await DbContext.SaveChangesAsync();
        return template;
    }

    protected async Task<Competition> CreateCompetitionAsync(
        string name = "Test Competition",
        string description = "Test Description",
        int maxMembers = 3,
        int maxExercises = 3,
        int maxSubmissionSize = 10000,
        TimeSpan? duration = null,
        TimeSpan? penalty = null)
    {
        duration ??= TimeSpan.FromHours(2);
        penalty ??= TimeSpan.FromMinutes(10);

        var now = DateTime.UtcNow;
        var template = Competition.CreateTemplate(
            name,
            description,
            now.AddDays(-1),
            now.AddDays(1),
            now.AddDays(2));

        DbContext.Competitions.Add(template);
        await DbContext.SaveChangesAsync();

        template.PromoteToCompetition(
            maxMembers,
            maxExercises,
            maxSubmissionSize,
            duration.Value,
            TimeSpan.FromMinutes(30),
            TimeSpan.FromMinutes(15),
            penalty.Value);

        await DbContext.SaveChangesAsync();
        return template;
    }

    protected async Task<ExerciseType> GetOrCreateExerciseTypeAsync(string label = "Programming")
    {
        var exerciseType = await DbContext.ExerciseTypes
            .FirstOrDefaultAsync(et => et.Label == label);

        if (exerciseType == null)
        {
            // Get max ID and add 1
            var maxId = await DbContext.ExerciseTypes.MaxAsync(et => (int?)et.Id) ?? 0;
            exerciseType = new ExerciseType(maxId + 1, label);
            DbContext.ExerciseTypes.Add(exerciseType);
            await DbContext.SaveChangesAsync();
        }

        return exerciseType;
    }

    protected async Task<Exercise> CreateExerciseAsync(
        string title = "Test Exercise",
        string? description = "Test Description",
        int? exerciseTypeId = null,
        TimeSpan? estimatedTime = null)
    {
        if (exerciseTypeId == null)
        {
            var exerciseType = await GetOrCreateExerciseTypeAsync();
            exerciseTypeId = exerciseType.Id;
        }

        estimatedTime ??= TimeSpan.FromMinutes(30);

        var exercise = new Exercise(title, description, exerciseTypeId.Value, estimatedTime.Value);
        DbContext.Exercises.Add(exercise);
        await DbContext.SaveChangesAsync();
        return exercise;
    }

    protected async Task CleanDatabaseAsync()
    {
        // Clear the change tracker to avoid conflicts with entities removed during the test
        DbContext.ChangeTracker.Clear();
        
        DbContext.Logs.RemoveRange(DbContext.Logs);
        DbContext.Answers.RemoveRange(DbContext.Answers);
        DbContext.Questions.RemoveRange(DbContext.Questions);
        DbContext.GroupExerciseAttempts.RemoveRange(DbContext.GroupExerciseAttempts);
        DbContext.ExerciseOutputs.RemoveRange(DbContext.ExerciseOutputs);
        DbContext.ExerciseInputs.RemoveRange(DbContext.ExerciseInputs);
        DbContext.ExercisesInCompetition.RemoveRange(DbContext.ExercisesInCompetition);
        DbContext.Exercises.RemoveRange(DbContext.Exercises);
        DbContext.CompetitionRankings.RemoveRange(DbContext.CompetitionRankings);
        DbContext.GroupsInCompetitions.RemoveRange(DbContext.GroupsInCompetitions);
        DbContext.Competitions.RemoveRange(DbContext.Competitions);
        DbContext.GroupInvites.RemoveRange(DbContext.GroupInvites);
        DbContext.Groups.RemoveRange(DbContext.Groups);
        DbContext.Users.RemoveRange(DbContext.Users);
        DbContext.AttachedFiles.RemoveRange(DbContext.AttachedFiles);
        await DbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        // Clean database after each test to prevent interference between tests
        CleanDatabaseAsync().GetAwaiter().GetResult();
        Scope?.Dispose();
        HttpClient?.Dispose();
    }
}
