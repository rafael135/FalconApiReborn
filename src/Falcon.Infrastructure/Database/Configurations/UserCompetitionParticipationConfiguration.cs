using Falcon.Core.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// Entity Framework Core configuration for the <see cref="UserCompetitionParticipation"/> entity.
/// </summary>
public class UserCompetitionParticipationConfiguration
    : IEntityTypeConfiguration<UserCompetitionParticipation>
{
    /// <summary>
    /// Configures the entity of type <see cref="UserCompetitionParticipation"/>.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<UserCompetitionParticipation> builder)
    {
        builder.ToTable("UserCompetitionParticipations");

        builder.HasKey(ucp => ucp.Id);

        builder.Property(ucp => ucp.UserId).IsRequired().HasMaxLength(450);

        builder.Property(ucp => ucp.CompetitionId).IsRequired();

        builder.Property(ucp => ucp.GroupId).IsRequired();

        builder.Property(ucp => ucp.JoinedAt).IsRequired();

        // Relationships
        builder
            .HasOne(ucp => ucp.User)
            .WithMany()
            .HasForeignKey(ucp => ucp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(ucp => ucp.Competition)
            .WithMany()
            .HasForeignKey(ucp => ucp.CompetitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(ucp => ucp.Group)
            .WithMany()
            .HasForeignKey(ucp => ucp.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // Composite index for efficient queries
        builder.HasIndex(ucp => new { ucp.CompetitionId, ucp.UserId }).IsUnique();

        builder.HasIndex(ucp => ucp.UserId);
    }
}
