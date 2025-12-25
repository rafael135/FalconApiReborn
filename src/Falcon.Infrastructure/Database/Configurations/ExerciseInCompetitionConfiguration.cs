using Falcon.Core.Domain.Competitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class ExerciseInCompetitionConfiguration : IEntityTypeConfiguration<ExerciseInCompetition>
{
    public void Configure(EntityTypeBuilder<ExerciseInCompetition> builder)
    {
        builder.HasKey(e => new { e.CompetitionId, e.ExerciseId });

        builder
            .HasOne(e => e.Competition)
            .WithMany(c => c.ExercisesInCompetition)
            .HasForeignKey(e => e.CompetitionId);

        builder.HasOne(e => e.Exercise).WithMany().HasForeignKey(e => e.ExerciseId);
    }
}
