using Falcon.Core.Domain.Competitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

/// <summary>
/// EF Core configuration for the <see cref="CompetitionRanking"/> entity.
/// </summary>
public class CompetitionRankingConfiguration : IEntityTypeConfiguration<CompetitionRanking>
{
    /// <summary>
    /// Configures the entity mapping for <see cref="CompetitionRanking"/>.
    /// </summary>
    public void Configure(EntityTypeBuilder<CompetitionRanking> builder)
    {
        builder.ToTable("CompetitionRankings");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Id)
            .ValueGeneratedOnAdd();

        builder.Property(cr => cr.Points)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cr => cr.Penalty)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(cr => cr.RankOrder)
            .IsRequired();

        // Relacionamentos
        builder.HasOne(cr => cr.Competition)
            .WithMany(c => c.Rankings)
            .HasForeignKey(cr => cr.CompetitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cr => cr.Group)
            .WithMany(g => g.Rankings)
            .HasForeignKey(cr => cr.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(cr => new { cr.CompetitionId, cr.GroupId })
            .IsUnique();

        builder.HasIndex(cr => new { cr.CompetitionId, cr.RankOrder });
    }
}
