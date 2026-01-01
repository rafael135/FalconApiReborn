using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class GroupExerciseAttemptConfiguration : IEntityTypeConfiguration<GroupExerciseAttempt>
{
    public void Configure(EntityTypeBuilder<GroupExerciseAttempt> builder)
    {
        builder.ToTable("GroupExerciseAttempts");

        builder.HasKey(gea => gea.Id);

        builder.Property(gea => gea.Id)
            .ValueGeneratedOnAdd();

        builder.Property(gea => gea.Code)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(gea => gea.Time)
            .IsRequired();

        builder.Property(gea => gea.SubmissionTime)
            .IsRequired();

        builder.Property(gea => gea.Language)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(gea => gea.Accepted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(gea => gea.JudgeResponse)
            .IsRequired()
            .HasConversion<int>();

        // Relacionamentos
        builder.HasOne(gea => gea.Exercise)
            .WithMany(e => e.Attempts)
            .HasForeignKey(gea => gea.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(gea => gea.Group)
            .WithMany(g => g.Attempts)
            .HasForeignKey(gea => gea.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gea => gea.Competition)
            .WithMany(c => c.Attempts)
            .HasForeignKey(gea => gea.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(gea => new { gea.CompetitionId, gea.GroupId, gea.ExerciseId });
        builder.HasIndex(gea => gea.SubmissionTime);
    }
}
