using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class CustomEmojiEntityConfiguration : IEntityTypeConfiguration<CustomEmoji>
{
    public void Configure(EntityTypeBuilder<CustomEmoji> builder)
    {
        builder.ToTable("custom_emojis");
        
        builder.HasKey(e => e.Id).HasName("custom_emojis_pkey");

        builder.HasIndex(e => new { e.Shortcode, e.Domain })
            .HasDatabaseName("index_custom_emojis_on_shortcode_and_domain")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CategoryId).HasColumnName("category_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Disabled).HasColumnName("disabled");

        builder.Property(e => e.Domain)
            .HasColumnType("character varying")
            .HasColumnName("domain");

        builder.Property(e => e.ImageContentType)
            .HasColumnType("character varying")
            .HasColumnName("image_content_type");

        builder.Property(e => e.ImageFileName)
            .HasColumnType("character varying")
            .HasColumnName("image_file_name");

        builder.Property(e => e.ImageFileSize).HasColumnName("image_file_size");

        builder.Property(e => e.ImageRemoteUrl)
            .HasColumnType("character varying")
            .HasColumnName("image_remote_url");

        builder.Property(e => e.ImageStorageSchemaVersion).HasColumnName("image_storage_schema_version");

        builder.Property(e => e.ImageUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("image_updated_at");

        builder.Property(e => e.Shortcode)
            .HasColumnType("character varying")
            .HasColumnName("shortcode")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Uri)
            .HasColumnType("character varying")
            .HasColumnName("uri");

        builder.Property(e => e.VisibleInPicker)
            .IsRequired()
            .HasColumnName("visible_in_picker")
            .HasDefaultValueSql("true");
    }
}