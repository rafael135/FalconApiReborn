using Falcon.Core.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="Group"/> entity.
/// </summary>
public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="Group"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.ToTable("Groups");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id)
            .ValueGeneratedOnAdd();

        builder.Property(g => g.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(g => g.LeaderId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(g => g.RowVersion)
            .IsRowVersion();

        builder.HasMany(g => g.Users)
            .WithOne(u => u.Group)
            .HasForeignKey(u => u.GroupId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
