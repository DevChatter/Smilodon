using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class SiteUploadEntityConfiguration : IEntityTypeConfiguration<SiteUpload>
{
    public void Configure(EntityTypeBuilder<SiteUpload> builder)
    {
        builder.ToTable("site_uploads");
        
        builder.HasKey(e => e.Id).HasName("site_uploads_pkey");

        builder.HasIndex(e => e.Var)
            .HasDatabaseName("index_site_uploads_on_var")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Blurhash)
            .HasColumnType("character varying")
            .HasColumnName("blurhash");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.FileContentType)
            .HasColumnType("character varying")
            .HasColumnName("file_content_type");

        builder.Property(e => e.FileFileName)
            .HasColumnType("character varying")
            .HasColumnName("file_file_name");

        builder.Property(e => e.FileFileSize).HasColumnName("file_file_size");

        builder.Property(e => e.FileUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("file_updated_at");

        builder.Property(e => e.Meta)
            .HasColumnType("json")
            .HasColumnName("meta");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Var)
            .HasColumnType("character varying")
            .HasColumnName("var")
            .HasDefaultValueSql("''::character varying");
    }
}