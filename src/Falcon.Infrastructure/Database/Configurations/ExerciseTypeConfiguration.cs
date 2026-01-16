using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="ExerciseType"/> entity.
/// </summary>
public class ExerciseTypeConfiguration : IEntityTypeConfiguration<ExerciseType>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="ExerciseType"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<ExerciseType> builder)
    {
        builder.ToTable("ExerciseTypes");

        builder.HasKey(et => et.Id);

        builder.Property(et => et.Id)
            .ValueGeneratedNever();

        builder.Property(et => et.Label)
            .HasMaxLength(100)
            .IsRequired();
    }
}
