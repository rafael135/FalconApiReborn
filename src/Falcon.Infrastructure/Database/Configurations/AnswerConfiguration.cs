using Falcon.Core.Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Answer"/> entity.
/// </summary>
public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="Answer"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).ValueGeneratedOnAdd();

        builder.Property(a => a.Content).IsRequired().HasMaxLength(Answer.MaxContentLength);

        builder.Property(a => a.UserId).IsRequired().HasMaxLength(450);

        builder.Property(a => a.CreatedAt).IsRequired();

        // Relacionamentos
        builder
            .HasOne(a => a.User)
            .WithMany(u => u.Answers)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Question relationship is configured in QuestionConfiguration
    }
}
