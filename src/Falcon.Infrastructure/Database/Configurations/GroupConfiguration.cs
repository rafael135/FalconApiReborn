using Falcon.Core.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name).HasMaxLength(100).IsRequired();

        builder.Property(g => g.RowVersion).IsRowVersion();

        builder.HasMany(g => g.Users).WithOne(u => u.Group).HasForeignKey(u => u.GroupId);
    }
}
