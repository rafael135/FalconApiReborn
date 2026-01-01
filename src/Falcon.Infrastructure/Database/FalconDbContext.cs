using Falcon.Core.Domain.Auditing;
using Falcon.Core.Domain.Competitions;
using Falcon.Core.Domain.Exercises;
using Falcon.Core.Domain.Files;
using Falcon.Core.Domain.Groups;
using Falcon.Core.Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Falcon.Infrastructure.Database;

public class FalconDbContext : IdentityDbContext<User>
{
    public FalconDbContext(DbContextOptions<FalconDbContext> options)
        : base(options) { }

    // Groups
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupInvite> GroupInvites { get; set; }
    public DbSet<GroupInCompetition> GroupsInCompetitions { get; set; }

    // Competitions
    public DbSet<Competition> Competitions { get; set; }
    public DbSet<CompetitionRanking> CompetitionRankings { get; set; }

    // Exercises
    public DbSet<ExerciseType> ExerciseTypes { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<ExerciseInCompetition> ExercisesInCompetition { get; set; }
    public DbSet<ExerciseInput> ExerciseInputs { get; set; }
    public DbSet<ExerciseOutput> ExerciseOutputs { get; set; }
    public DbSet<GroupExerciseAttempt> GroupExerciseAttempts { get; set; }

    // Questions & Answers
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }

    // Shared
    public DbSet<AttachedFile> AttachedFiles { get; set; }
    public DbSet<Log> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        // This assumes that entity configurations are defined in separate classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FalconDbContext).Assembly);
    }
}
