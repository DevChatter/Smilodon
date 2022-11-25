using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AnnouncementReactionEntityConfiguration : IEntityTypeConfiguration<AnnouncementReaction>
{
    public void Configure(EntityTypeBuilder<AnnouncementReaction> builder)
    {
        builder.ToTable("announcement_reactions");
        
        builder.HasKey(e => e.Id).HasName("announcement_reactions_pkey");

        builder.HasIndex(e => new { e.AccountId, e.AnnouncementId, e.Name })
            .HasDatabaseName("index_announcement_reactions_on_account_id_and_announcement_id")
            .IsUnique();

        builder.HasIndex(e => e.AnnouncementId).HasDatabaseName("index_announcement_reactions_on_announcement_id");

        builder.HasIndex(e => e.CustomEmojiId)
            .HasDatabaseName("index_announcement_reactions_on_custom_emoji_id")
            .HasFilter("(custom_emoji_id IS NOT NULL)");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.AnnouncementId).HasColumnName("announcement_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.CustomEmojiId).HasColumnName("custom_emoji_id");

        builder.Property(e => e.Name)
            .HasColumnType("character varying")
            .HasColumnName("name")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AnnouncementReactions)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_7444ad831f");

        builder.HasOne(d => d.Announcement)
            .WithMany(p => p.AnnouncementReactions)
            .HasForeignKey(d => d.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_a1226eaa5c");

        builder.HasOne(d => d.CustomEmoji)
            .WithMany(p => p.AnnouncementReactions)
            .HasForeignKey(d => d.CustomEmojiId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_b742c91c0e");
    }
}