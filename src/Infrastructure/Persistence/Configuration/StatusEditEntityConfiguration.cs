using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class StatusEditEntityConfiguration : IEntityTypeConfiguration<StatusEdit>
{
    public void Configure(EntityTypeBuilder<StatusEdit> builder)
    {
        builder.ToTable("status_edits");
        
        builder.HasKey(e => e.Id).HasName("status_edits_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_status_edits_on_account_id");

        builder.HasIndex(e => e.StatusId).HasDatabaseName("index_status_edits_on_status_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.MediaDescriptions).HasColumnName("media_descriptions");

        builder.Property(e => e.OrderedMediaAttachmentIds).HasColumnName("ordered_media_attachment_ids");

        builder.Property(e => e.PollOptions)
            .HasColumnType("character varying[]")
            .HasColumnName("poll_options");

        builder.Property(e => e.Sensitive).HasColumnName("sensitive");

        builder.Property(e => e.SpoilerText)
            .HasColumnName("spoiler_text")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.Property(e => e.Text)
            .HasColumnName("text")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.StatusEdits)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_dc8988c545");

        builder.HasOne(d => d.Status)
            .WithMany(p => p.StatusEdits)
            .HasForeignKey(d => d.StatusId)
            .HasConstraintName("fk_rails_a960f234a0");
    }
}