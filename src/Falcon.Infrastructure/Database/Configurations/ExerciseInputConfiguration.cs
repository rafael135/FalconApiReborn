using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class ExerciseInputConfiguration : IEntityTypeConfiguration<ExerciseInput>
{
    public void Configure(EntityTypeBuilder<ExerciseInput> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExerciseId).IsRequired(true);
        builder.Property(e => e.InputContent).IsRequired(true);
        builder.Property(e => e.JudgeUuid).IsRequired(true);

        builder
            .HasOne(e => e.Output)
            .WithOne()
            .HasForeignKey<ExerciseOutput>(o => o.ExerciseInputId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
