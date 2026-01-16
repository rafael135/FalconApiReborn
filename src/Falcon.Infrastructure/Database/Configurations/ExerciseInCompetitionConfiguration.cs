using Falcon.Core.Domain.Competitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="ExerciseInCompetition"/> join entity.
/// </summary>
public class ExerciseInCompetitionConfiguration : IEntityTypeConfiguration<ExerciseInCompetition>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="ExerciseInCompetition"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<ExerciseInCompetition> builder)
    {
        builder.ToTable("ExercisesInCompetitions");

        builder.HasKey(eic => new { eic.CompetitionId, eic.ExerciseId });

        builder
            .HasOne(eic => eic.Competition)
            .WithMany(c => c.ExercisesInCompetition)
            .HasForeignKey(eic => eic.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(eic => eic.Exercise)
            .WithMany()
            .HasForeignKey(eic => eic.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
