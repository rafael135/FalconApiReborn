using Falcon.Core.Domain.Competitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class CompetitionConfiguration : IEntityTypeConfiguration<Competition>
{
    public void Configure(EntityTypeBuilder<Competition> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).HasMaxLength(200).IsRequired(true);
        builder.Property(c => c.Description).HasMaxLength(1000).IsRequired(true);
        builder.Property(c => c.StartInscriptions).IsRequired(true);
        builder.Property(c => c.EndInscriptions).IsRequired(true);
        builder.Property(c => c.StartTime).IsRequired(true);
        builder.Property(c => c.EndTime).IsRequired(false);
        builder.Property(c => c.Duration).IsRequired(false);
        builder.Property(c => c.BlockSubmissions).IsRequired(false);
        builder.Property(c => c.StopRanking).IsRequired(false);
        builder.Property(c => c.MaxExercises).IsRequired(false);
        builder.Property(c => c.MaxMembers).IsRequired(false);
        builder.Property(c => c.MaxSubmissionSize).IsRequired(false);
        builder.Property(c => c.SubmissionPenalty).IsRequired(false);
        builder.Property(c => c.Status);

        builder
            .HasMany(c => c.ExercisesInCompetition)
            .WithOne(e => e.Competition)
            .HasForeignKey(e => e.CompetitionId);
    }
}
