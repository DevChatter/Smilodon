using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class StatusEntityConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("statuses");
        
        builder.HasKey(e => e.Id).HasName("statuses_pkey");

        builder.HasIndex(e => new { e.AccountId, e.Id, e.Visibility, e.UpdatedAt })
            .HasDatabaseName("index_statuses_20190820")
            .HasFilter("(deleted_at IS NULL)")
            .IsDescending(false, true, false, false);
        
        builder.HasIndex(e => new { e.Id, e.AccountId })
            .HasDatabaseName("index_statuses_local_20190824")
            .HasFilter(
                "((local OR (uri IS NULL)) AND (deleted_at IS NULL) AND (visibility = 0) AND (reblog_of_id IS NULL) AND ((NOT reply) OR (in_reply_to_account_id = account_id)))")
            .IsDescending(true, false);

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_statuses_on_account_id");

        builder.HasIndex(e => e.DeletedAt)
            .HasDatabaseName("index_statuses_on_deleted_at")
            .HasFilter("(deleted_at IS NOT NULL)");

        builder.HasIndex(e => e.InReplyToAccountId)
            .HasDatabaseName("index_statuses_on_in_reply_to_account_id")
            .HasFilter("(in_reply_to_account_id IS NOT NULL)");

        builder.HasIndex(e => e.InReplyToId)
            .HasDatabaseName("index_statuses_on_in_reply_to_id")
            .HasFilter("(in_reply_to_id IS NOT NULL)");

        builder.HasIndex(e => new { e.ReblogOfId, e.AccountId })
            .HasDatabaseName("index_statuses_on_reblog_of_id_and_account_id");

        builder.HasIndex(e => e.Uri)
            .HasDatabaseName("index_statuses_on_uri")
            .IsUnique()
            .HasFilter("(uri IS NOT NULL)")
            .HasOperators(new[] { "text_pattern_ops" });

        builder.HasIndex(e => new { e.Id, e.AccountId })
            .HasDatabaseName("index_statuses_public_20200119")
            .HasFilter(
                "((deleted_at IS NULL) AND (visibility = 0) AND (reblog_of_id IS NULL) AND ((NOT reply) OR (in_reply_to_account_id = account_id)))")
            .IsDescending(true, false);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("timestamp_id('statuses'::text)");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.ApplicationId).HasColumnName("application_id");

        builder.Property(e => e.ConversationId).HasColumnName("conversation_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.DeletedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("deleted_at");

        builder.Property(e => e.EditedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("edited_at");

        builder.Property(e => e.InReplyToAccountId).HasColumnName("in_reply_to_account_id");

        builder.Property(e => e.InReplyToId).HasColumnName("in_reply_to_id");

        builder.Property(e => e.Language)
            .HasColumnType("character varying")
            .HasColumnName("language");

        builder.Property(e => e.Local).HasColumnName("local");

        builder.Property(e => e.OrderedMediaAttachmentIds).HasColumnName("ordered_media_attachment_ids");

        builder.Property(e => e.PollId).HasColumnName("poll_id");

        builder.Property(e => e.ReblogOfId).HasColumnName("reblog_of_id");

        builder.Property(e => e.Reply).HasColumnName("reply");

        builder.Property(e => e.Sensitive).HasColumnName("sensitive");

        builder.Property(e => e.SpoilerText)
            .HasColumnName("spoiler_text")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.Text)
            .HasColumnName("text")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.Trendable).HasColumnName("trendable");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Uri)
            .HasColumnType("character varying")
            .HasColumnName("uri");

        builder.Property(e => e.Url)
            .HasColumnType("character varying")
            .HasColumnName("url");

        builder.Property(e => e.Visibility).HasColumnName("visibility");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.StatusAccounts)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_9bda1543f7");

        builder.HasOne(d => d.InReplyToAccount)
            .WithMany(p => p.StatusInReplyToAccounts)
            .HasForeignKey(d => d.InReplyToAccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_c7fa917661");

        builder.HasOne(d => d.InReplyTo)
            .WithMany(p => p.InverseInReplyTo)
            .HasForeignKey(d => d.InReplyToId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_94a6f70399");

        builder.HasOne(d => d.ReblogOf)
            .WithMany(p => p.InverseReblogOf)
            .HasForeignKey(d => d.ReblogOfId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_256483a9ab");
    }
}