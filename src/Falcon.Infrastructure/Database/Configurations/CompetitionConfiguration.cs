using Falcon.Core.Domain.Competitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Competition"/> entity.
/// </summary>
public class CompetitionConfiguration : IEntityTypeConfiguration<Competition>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="Competition"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<Competition> builder)
    {
        builder.ToTable("Competitions");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(c => c.StartInscriptions)
            .IsRequired();

        builder.Property(c => c.EndInscriptions)
            .IsRequired();

        builder.Property(c => c.StartTime)
            .IsRequired();

        builder.Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>();

        builder
            .HasMany(c => c.ExercisesInCompetition)
            .WithOne(e => e.Competition)
            .HasForeignKey(e => e.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
