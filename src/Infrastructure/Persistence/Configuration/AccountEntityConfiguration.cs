using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountEntityConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("accounts");
        
        builder.HasKey(e => e.Id).HasName("accounts_pkey");

        builder.HasIndex(e => e.MovedToAccountId)
            .HasDatabaseName("index_accounts_on_moved_to_account_id")
            .HasFilter("(moved_to_account_id IS NOT NULL)");

        builder.HasIndex(e => e.Uri).HasDatabaseName("index_accounts_on_uri");

        builder.HasIndex(e => e.Url)
            .HasDatabaseName("index_accounts_on_url")
            .HasFilter("(url IS NOT NULL)")
            .HasOperators("text_pattern_ops");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("timestamp_id('accounts'::text)");

        builder.Property(e => e.ActorType)
            .HasColumnType("character varying")
            .HasColumnName("actor_type");

        builder.Property(e => e.AlsoKnownAs)
            .HasColumnType("character varying[]")
            .HasColumnName("also_known_as");

        builder.Property(e => e.AvatarContentType)
            .HasColumnType("character varying")
            .HasColumnName("avatar_content_type");

        builder.Property(e => e.AvatarFileName)
            .HasColumnType("character varying")
            .HasColumnName("avatar_file_name");

        builder.Property(e => e.AvatarFileSize).HasColumnName("avatar_file_size");

        builder.Property(e => e.AvatarRemoteUrl)
            .HasColumnType("character varying")
            .HasColumnName("avatar_remote_url");

        builder.Property(e => e.AvatarStorageSchemaVersion).HasColumnName("avatar_storage_schema_version");

        builder.Property(e => e.AvatarUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("avatar_updated_at");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.DevicesUrl)
            .HasColumnType("character varying")
            .HasColumnName("devices_url");

        builder.Property(e => e.Discoverable).HasColumnName("discoverable");

        builder.Property(e => e.DisplayName)
            .HasColumnType("character varying")
            .HasColumnName("display_name")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Domain)
            .HasColumnType("character varying")
            .HasColumnName("domain");

        builder.Property(e => e.FeaturedCollectionUrl)
            .HasColumnType("character varying")
            .HasColumnName("featured_collection_url");

        builder.Property(e => e.Fields)
            .HasColumnType("jsonb")
            .HasColumnName("fields");

        builder.Property(e => e.FollowersUrl)
            .HasColumnType("character varying")
            .HasColumnName("followers_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.HeaderContentType)
            .HasColumnType("character varying")
            .HasColumnName("header_content_type");

        builder.Property(e => e.HeaderFileName)
            .HasColumnType("character varying")
            .HasColumnName("header_file_name");

        builder.Property(e => e.HeaderFileSize).HasColumnName("header_file_size");

        builder.Property(e => e.HeaderRemoteUrl)
            .HasColumnType("character varying")
            .HasColumnName("header_remote_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.HeaderStorageSchemaVersion).HasColumnName("header_storage_schema_version");

        builder.Property(e => e.HeaderUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("header_updated_at");

        builder.Property(e => e.HideCollections).HasColumnName("hide_collections");

        builder.Property(e => e.InboxUrl)
            .HasColumnType("character varying")
            .HasColumnName("inbox_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.LastWebfingeredAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("last_webfingered_at");

        builder.Property(e => e.Locked).HasColumnName("locked");

        builder.Property(e => e.Memorial).HasColumnName("memorial");

        builder.Property(e => e.MovedToAccountId).HasColumnName("moved_to_account_id");

        builder.Property(e => e.Note)
            .HasColumnName("note")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.OutboxUrl)
            .HasColumnType("character varying")
            .HasColumnName("outbox_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.PrivateKey).HasColumnName("private_key");

        builder.Property(e => e.Protocol).HasColumnName("protocol");

        builder.Property(e => e.PublicKey)
            .HasColumnName("public_key")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.RequestedReviewAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("requested_review_at");

        builder.Property(e => e.ReviewedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("reviewed_at");

        builder.Property(e => e.SensitizedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("sensitized_at");

        builder.Property(e => e.SharedInboxUrl)
            .HasColumnType("character varying")
            .HasColumnName("shared_inbox_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.SilencedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("silenced_at");

        builder.Property(e => e.SuspendedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("suspended_at");

        builder.Property(e => e.SuspensionOrigin).HasColumnName("suspension_origin");

        builder.Property(e => e.Trendable).HasColumnName("trendable");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Uri)
            .HasColumnType("character varying")
            .HasColumnName("uri")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Url)
            .HasColumnType("character varying")
            .HasColumnName("url");

        builder.Property(e => e.Username)
            .HasColumnType("character varying")
            .HasColumnName("username")
            .HasDefaultValueSql("''::character varying");

        builder.HasOne(d => d.MovedToAccount)
            .WithMany(p => p.InverseMovedToAccount)
            .HasForeignKey(d => d.MovedToAccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_2320833084");
    }
}