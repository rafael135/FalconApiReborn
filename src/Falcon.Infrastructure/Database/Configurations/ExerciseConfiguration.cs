using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Exercise"/> entity.
/// </summary>
public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="Exercise"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercises");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.Title).HasMaxLength(200).IsRequired();

        builder.Property(e => e.Description).HasColumnType("nvarchar(max)");

        builder.Property(e => e.ExerciseTypeId).IsRequired();

        builder.Property(e => e.EstimatedTime).IsRequired();

        builder.Property(e => e.JudgeUuid).HasMaxLength(36);

        builder.Property(e => e.CreatedAt).IsRequired().HasDefaultValueSql("GETUTCDATE()");

        builder
            .HasOne(e => e.ExerciseType)
            .WithMany(et => et.Exercises)
            .HasForeignKey(e => e.ExerciseTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(e => e.AttachedFile)
            .WithMany(af => af.Exercises)
            .HasForeignKey(e => e.AttachedFileId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(e => e.Inputs)
            .WithOne()
            .HasForeignKey(i => i.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(e => e.Outputs)
            .WithOne(o => o.Exercise)
            .HasForeignKey(o => o.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
