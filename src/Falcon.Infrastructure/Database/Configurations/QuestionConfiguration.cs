using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id)
            .ValueGeneratedOnAdd();

        builder.Property(q => q.Content)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(q => q.QuestionType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.Property(q => q.UserId)
            .IsRequired()
            .HasMaxLength(450);

        // Relacionamentos
        builder.HasOne(q => q.Competition)
            .WithMany(c => c.Questions)
            .HasForeignKey(q => q.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(q => q.Exercise)
            .WithMany(e => e.Questions)
            .HasForeignKey(q => q.ExerciseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(q => q.User)
            .WithMany(u => u.Questions)
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.Answer)
            .WithOne(a => a.Question)
            .HasForeignKey<Question>(q => q.AnswerId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(q => q.CompetitionId);
        builder.HasIndex(q => q.CreatedAt);
    }
}
