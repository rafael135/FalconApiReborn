using Falcon.Core.Domain.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falcon.Infrastructure.Database.Configurations;

public class AttachedFileConfiguration : IEntityTypeConfiguration<AttachedFile>
{
    public void Configure(EntityTypeBuilder<AttachedFile> builder)
    {
        builder.ToTable("AttachedFiles");

        builder.HasKey(af => af.Id);

        builder.Property(af => af.Id)
            .ValueGeneratedOnAdd();

        builder.Property(af => af.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(af => af.Type)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(af => af.Size)
            .IsRequired();

        builder.Property(af => af.FilePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(af => af.CreatedAt)
            .IsRequired();

        // Relacionamento muitos-para-muitos com Exercise será configurado no ExerciseConfiguration
    }
}
