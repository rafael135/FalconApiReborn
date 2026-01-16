using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="ExerciseInput"/> entity.
/// </summary>
public class ExerciseInputConfiguration : IEntityTypeConfiguration<ExerciseInput>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="ExerciseInput"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<ExerciseInput> builder)
    {
        builder.ToTable("ExerciseInputs");

        builder.HasKey(ei => ei.Id);

        builder.Property(ei => ei.Id).ValueGeneratedOnAdd();

        builder.Property(ei => ei.ExerciseId).IsRequired();

        builder.Property(ei => ei.InputContent).IsRequired().HasColumnType("nvarchar(max)");

        builder.Property(ei => ei.JudgeUuid).HasMaxLength(36);

        // Relacionamento 1:1 configurado no ExerciseOutputConfiguration
    }
}
