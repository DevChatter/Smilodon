using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class MediaAttachmentEntityConfiguration : IEntityTypeConfiguration<MediaAttachment>
{
    public void Configure(EntityTypeBuilder<MediaAttachment> builder)
    {
        builder.ToTable("media_attachments");
        
        builder.HasKey(e => e.Id).HasName("media_attachments_pkey");

        builder.HasIndex(e => new { e.AccountId, e.StatusId })
            .HasDatabaseName("index_media_attachments_on_account_id_and_status_id")
            .IsDescending(false, true);

        builder.HasIndex(e => e.ScheduledStatusId)
            .HasDatabaseName("index_media_attachments_on_scheduled_status_id")
            .HasFilter("(scheduled_status_id IS NOT NULL)");

        builder.HasIndex(e => e.Shortcode)
            .HasDatabaseName("index_media_attachments_on_shortcode")
            .IsUnique()
            .HasFilter("(shortcode IS NOT NULL)")
            .HasOperators("text_pattern_ops");

        builder.HasIndex(e => e.StatusId).HasDatabaseName("index_media_attachments_on_status_id");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("timestamp_id('media_attachments'::text)");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Blurhash)
            .HasColumnType("character varying")
            .HasColumnName("blurhash");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Description).HasColumnName("description");

        builder.Property(e => e.FileContentType)
            .HasColumnType("character varying")
            .HasColumnName("file_content_type");

        builder.Property(e => e.FileFileName)
            .HasColumnType("character varying")
            .HasColumnName("file_file_name");

        builder.Property(e => e.FileFileSize).HasColumnName("file_file_size");

        builder.Property(e => e.FileMeta)
            .HasColumnType("json")
            .HasColumnName("file_meta");

        builder.Property(e => e.FileStorageSchemaVersion).HasColumnName("file_storage_schema_version");

        builder.Property(e => e.FileUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("file_updated_at");

        builder.Property(e => e.Processing).HasColumnName("processing");

        builder.Property(e => e.RemoteUrl)
            .HasColumnType("character varying")
            .HasColumnName("remote_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.ScheduledStatusId).HasColumnName("scheduled_status_id");

        builder.Property(e => e.Shortcode)
            .HasColumnType("character varying")
            .HasColumnName("shortcode");

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.Property(e => e.ThumbnailContentType)
            .HasColumnType("character varying")
            .HasColumnName("thumbnail_content_type");

        builder.Property(e => e.ThumbnailFileName)
            .HasColumnType("character varying")
            .HasColumnName("thumbnail_file_name");

        builder.Property(e => e.ThumbnailFileSize).HasColumnName("thumbnail_file_size");

        builder.Property(e => e.ThumbnailRemoteUrl)
            .HasColumnType("character varying")
            .HasColumnName("thumbnail_remote_url");

        builder.Property(e => e.ThumbnailUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("thumbnail_updated_at");

        builder.Property(e => e.Type).HasColumnName("type");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.MediaAttachments)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_96dd81e81b");

        builder.HasOne(d => d.ScheduledStatus)
            .WithMany(p => p.MediaAttachments)
            .HasForeignKey(d => d.ScheduledStatusId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_31fc5aeef1");

        builder.HasOne(d => d.Status)
            .WithMany(p => p.MediaAttachments)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_3ec0cfdd70");
    }
}