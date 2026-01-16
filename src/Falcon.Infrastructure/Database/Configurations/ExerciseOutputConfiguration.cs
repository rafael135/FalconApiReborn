using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="ExerciseOutput"/> entity.
/// </summary>
public class ExerciseOutputConfiguration : IEntityTypeConfiguration<ExerciseOutput>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="ExerciseOutput"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<ExerciseOutput> builder)
    {
        builder.ToTable("ExerciseOutputs");

        builder.HasKey(eo => eo.Id);

        builder.Property(eo => eo.Id)
            .ValueGeneratedOnAdd();

        builder.Property(eo => eo.ExerciseId)
            .IsRequired();

        builder.Property(eo => eo.ExerciseInputId)
            .IsRequired();

        builder.Property(eo => eo.OutputContent)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(eo => eo.JudgeUuid)
            .HasMaxLength(36);

        builder.HasOne(eo => eo.Exercise)
            .WithMany(e => e.Outputs)
            .HasForeignKey(eo => eo.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(eo => eo.ExerciseInput)
            .WithOne(ei => ei.Output)
            .HasForeignKey<ExerciseOutput>(eo => eo.ExerciseInputId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
