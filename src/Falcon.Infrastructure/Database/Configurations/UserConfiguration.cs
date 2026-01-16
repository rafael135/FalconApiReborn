using Falcon.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="User"/> (Identity user) entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="User"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("AspNetUsers");

        builder.Property(u => u.Name).HasMaxLength(255).IsRequired();

        builder.Property(u => u.RA).HasMaxLength(15).IsRequired();

        builder.HasIndex(u => u.RA).IsUnique();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Department).HasMaxLength(100);

        builder.Property(u => u.CreatedAt).IsRequired();

        builder.Property(u => u.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.Property(u => u.DeletedAt).IsRequired(false);

        builder
            .HasOne(u => u.Group)
            .WithMany(g => g.Users)
            .HasForeignKey(u => u.GroupId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
