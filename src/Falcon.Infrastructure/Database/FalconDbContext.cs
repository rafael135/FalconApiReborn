using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Files;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Infrastructure.Database;

/// <summary>
/// Entity Framework Core <see cref="DbContext"/> for Falcon application data, including Identity.
/// </summary>
public class FalconDbContext : IdentityDbContext<User>
{
    /// <summary>
    /// Initializes a new instance of <see cref="FalconDbContext"/> with the specified options.
    /// </summary>
    /// <param name="options">The context options.</param>
    public FalconDbContext(DbContextOptions<FalconDbContext> options)
        : base(options) { }

    // Groups
    /// <summary>Groups table.</summary>
    public DbSet<Group> Groups { get; set; }

    /// <summary>Group invites table.</summary>
    public DbSet<GroupInvite> GroupInvites { get; set; }

    /// <summary>Groups in competitions association table.</summary>
    public DbSet<GroupInCompetition> GroupsInCompetitions { get; set; }

    // Competitions
    /// <summary>Competitions table.</summary>
    public DbSet<Competition> Competitions { get; set; }

    /// <summary>Competition rankings table.</summary>
    public DbSet<CompetitionRanking> CompetitionRankings { get; set; }

    // Exercises
    /// <summary>Exercise types table.</summary>
    public DbSet<ExerciseType> ExerciseTypes { get; set; }

    /// <summary>Exercises table.</summary>
    public DbSet<Exercise> Exercises { get; set; }

    /// <summary>Exercises in competitions association table.</summary>
    public DbSet<ExerciseInCompetition> ExercisesInCompetition { get; set; }

    /// <summary>Exercise inputs table.</summary>
    public DbSet<ExerciseInput> ExerciseInputs { get; set; }

    /// <summary>Exercise outputs table.</summary>
    public DbSet<ExerciseOutput> ExerciseOutputs { get; set; }

    /// <summary>Group exercise attempts table.</summary>
    public DbSet<GroupExerciseAttempt> GroupExerciseAttempts { get; set; }

    // Questions & Answers
    /// <summary>Questions table.</summary>
    public DbSet<Question> Questions { get; set; }

    /// <summary>Answers table.</summary>
    public DbSet<Answer> Answers { get; set; }

    // Shared
    /// <summary>Attached files table.</summary>
    public DbSet<AttachedFile> AttachedFiles { get; set; }

    /// <summary>Application logs table.</summary>
    public DbSet<Log> Logs { get; set; }

    // Users
    /// <summary>User competition participations table.</summary>
    public DbSet<UserCompetitionParticipation> UserCompetitionParticipations { get; set; }

    /// <summary>
    /// Applies entity configurations and global filters.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        // This assumes that entity configurations are defined in separate classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FalconDbContext).Assembly);

        // Global query filter for soft delete on User
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
    }
}
