using Falcon.Core.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class GroupInviteConfiguration : IEntityTypeConfiguration<GroupInvite>
{
    public void Configure(EntityTypeBuilder<GroupInvite> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Accepted).IsRequired(true).HasDefaultValue(false);

        builder
            .HasOne(g => g.Group)
            .WithMany()
            .HasForeignKey(g => g.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(g => g.User)
            .WithMany()
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
