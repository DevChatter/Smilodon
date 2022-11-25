using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class PreviewCardEntityConfiguration : IEntityTypeConfiguration<PreviewCard>
{
    public void Configure(EntityTypeBuilder<PreviewCard> builder)
    {
        builder.ToTable("preview_cards");
        
        builder.HasKey(e => e.Id).HasName("preview_cards_pkey");

        builder.HasIndex(e => e.Url)
            .HasDatabaseName("index_preview_cards_on_url")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AuthorName)
            .HasColumnType("character varying")
            .HasColumnName("author_name")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.AuthorUrl)
            .HasColumnType("character varying")
            .HasColumnName("author_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Blurhash)
            .HasColumnType("character varying")
            .HasColumnName("blurhash");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Description)
            .HasColumnType("character varying")
            .HasColumnName("description")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.EmbedUrl)
            .HasColumnType("character varying")
            .HasColumnName("embed_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Height).HasColumnName("height");

        builder.Property(e => e.Html)
            .HasColumnName("html")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.ImageContentType)
            .HasColumnType("character varying")
            .HasColumnName("image_content_type");

        builder.Property(e => e.ImageFileName)
            .HasColumnType("character varying")
            .HasColumnName("image_file_name");

        builder.Property(e => e.ImageFileSize).HasColumnName("image_file_size");

        builder.Property(e => e.ImageStorageSchemaVersion).HasColumnName("image_storage_schema_version");

        builder.Property(e => e.ImageUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("image_updated_at");

        builder.Property(e => e.Language)
            .HasColumnType("character varying")
            .HasColumnName("language");

        builder.Property(e => e.LinkType).HasColumnName("link_type");

        builder.Property(e => e.MaxScore).HasColumnName("max_score");

        builder.Property(e => e.MaxScoreAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("max_score_at");

        builder.Property(e => e.ProviderName)
            .HasColumnType("character varying")
            .HasColumnName("provider_name")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.ProviderUrl)
            .HasColumnType("character varying")
            .HasColumnName("provider_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Title)
            .HasColumnType("character varying")
            .HasColumnName("title")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Trendable).HasColumnName("trendable");

        builder.Property(e => e.Type).HasColumnName("type");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Url)
            .HasColumnType("character varying")
            .HasColumnName("url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Width).HasColumnName("width");
    }
}