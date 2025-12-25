using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).HasMaxLength(200).IsRequired(true);
        builder.Property(e => e.Description).IsRequired(false);
        builder.Property(e => e.ExerciseTypeId).IsRequired(true);
        builder.Property(e => e.EstimatedTime).IsRequired(true);
        builder.Property(e => e.JudgeUuid).IsRequired(true);
        builder.Property(e => e.CreatedAt).IsRequired(true).HasDefaultValueSql("GETUTCDATE()");

        builder
            .HasOne(e => e.ExerciseType)
            .WithMany()
            .HasForeignKey(e => e.ExerciseTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(e => e.Inputs)
            .WithOne()
            .HasForeignKey(i => i.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasMany(e => e.Outputs)
            .WithOne()
            .HasForeignKey(o => o.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
