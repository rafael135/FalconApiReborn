using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class ExerciseTypeConfiguration : IEntityTypeConfiguration<ExerciseType>
{
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
