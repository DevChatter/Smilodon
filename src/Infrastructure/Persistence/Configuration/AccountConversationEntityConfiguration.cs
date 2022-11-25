using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountConversationEntityConfiguration : IEntityTypeConfiguration<AccountConversation>
{
    public void Configure(EntityTypeBuilder<AccountConversation> builder)
    {
        builder.ToTable("account_conversations");
        
        builder.HasKey(e => e.Id).HasName("account_conversations_pkey");

        builder.HasIndex(e => e.ConversationId)
            .HasDatabaseName("index_account_conversations_on_conversation_id");

        builder.HasIndex(e => new { e.AccountId, e.ConversationId, e.ParticipantAccountIds })
            .HasDatabaseName("index_unique_conversations")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.ConversationId).HasColumnName("conversation_id");

        builder.Property(e => e.LastStatusId).HasColumnName("last_status_id");

        builder.Property(e => e.LockVersion).HasColumnName("lock_version");

        builder.Property(e => e.ParticipantAccountIds)
            .HasColumnName("participant_account_ids")
            .HasDefaultValueSql("'{}'::bigint[]");

        builder.Property(e => e.StatusIds)
            .HasColumnName("status_ids")
            .HasDefaultValueSql("'{}'::bigint[]");

        builder.Property(e => e.Unread).HasColumnName("unread");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AccountConversations)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_6f5278b6e9");

        builder.HasOne(d => d.Conversation)
            .WithMany(p => p.AccountConversations)
            .HasForeignKey(d => d.ConversationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_1491654f9f");
    }
}