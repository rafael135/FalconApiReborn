using Falcon.Core.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="GroupInvite"/> entity.
/// </summary>
public class GroupInviteConfiguration : IEntityTypeConfiguration<GroupInvite>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="GroupInvite"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<GroupInvite> builder)
    {
        builder.ToTable("GroupInvites");

        builder.HasKey(gi => gi.Id);

        builder.Property(gi => gi.Id).ValueGeneratedOnAdd();

        builder.Property(gi => gi.Accepted).IsRequired().HasDefaultValue(false);

        builder.Property(gi => gi.UserId).IsRequired().HasMaxLength(450);

        builder
            .HasOne(gi => gi.Group)
            .WithMany(g => g.Invites)
            .HasForeignKey(gi => gi.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(gi => gi.User)
            .WithMany()
            .HasForeignKey(gi => gi.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índice único para evitar convites duplicados
        builder.HasIndex(gi => new { gi.GroupId, gi.UserId }).IsUnique();
    }
}
