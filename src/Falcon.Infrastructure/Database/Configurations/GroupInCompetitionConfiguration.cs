using Falcon.Core.Domain.Competitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class GroupInCompetitionConfiguration : IEntityTypeConfiguration<GroupInCompetition>
{
    public void Configure(EntityTypeBuilder<GroupInCompetition> builder)
    {
        builder.ToTable("GroupsInCompetitions");

        builder.HasKey(gc => new { gc.GroupId, gc.CompetitionId });

        builder.Property(gc => gc.GroupId).IsRequired();
        builder.Property(gc => gc.CompetitionId).IsRequired();

        builder.Property(gc => gc.CreatedOn).IsRequired();

        builder.Property(gc => gc.Blocked).IsRequired().HasDefaultValue(false);

        builder
            .HasOne(gc => gc.Group)
            .WithMany(g => g.GroupsInCompetitions)
            .HasForeignKey(gc => gc.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(gc => gc.Competition)
            .WithMany(c => c.GroupsInCompetitions)
            .HasForeignKey(gc => gc.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
