using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class ExerciseOutputConfiguration : IEntityTypeConfiguration<ExerciseOutput>
{
    public void Configure(EntityTypeBuilder<ExerciseOutput> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ExerciseId).IsRequired(true);
        builder.Property(e => e.ExerciseInputId).IsRequired(true);
        builder.Property(e => e.OutputContent).IsRequired(true);
        builder.Property(e => e.JudgeUuid).IsRequired(true);

        builder.HasOne(e => e.Exercise).WithMany(ex => ex.Outputs).HasForeignKey(e => e.ExerciseId).OnDelete(DeleteBehavior.NoAction);
    }
}
