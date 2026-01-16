using Falcon.Core.Domain.Auditing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Log"/> entity.
/// </summary>
public class LogConfiguration : IEntityTypeConfiguration<Log>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="Log"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable("Logs");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id).ValueGeneratedOnAdd();

        builder.Property(l => l.ActionType).IsRequired().HasConversion<int>();

        builder.Property(l => l.ActionTime).IsRequired();

        builder.Property(l => l.IpAddress).IsRequired().HasMaxLength(128);

        builder.Property(l => l.UserId).HasMaxLength(450);

        // Relacionamentos
        builder
            .HasOne(l => l.User)
            .WithMany(u => u.Logs)
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(l => l.Group)
            .WithMany(g => g.Logs)
            .HasForeignKey(l => l.GroupId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(l => l.Competition)
            .WithMany(c => c.Logs)
            .HasForeignKey(l => l.CompetitionId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(l => l.ActionTime);
        builder.HasIndex(l => l.UserId);
    }
}
