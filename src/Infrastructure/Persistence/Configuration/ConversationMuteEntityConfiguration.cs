using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class ConversationMuteEntityConfiguration : IEntityTypeConfiguration<ConversationMute>
{
    public void Configure(EntityTypeBuilder<ConversationMute> builder)
    {
        builder.ToTable("conversation_mutes");
        
        builder.HasKey(e => e.Id).HasName("conversation_mutes_pkey");

        builder.HasIndex(e => new { e.AccountId, e.ConversationId })
            .HasDatabaseName("index_conversation_mutes_on_account_id_and_conversation_id")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.ConversationId).HasColumnName("conversation_id");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.ConversationMutes)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_225b4212bb");

        builder.HasOne(d => d.Conversation)
            .WithMany(p => p.ConversationMutes)
            .HasForeignKey(d => d.ConversationId)
            .HasConstraintName("fk_rails_5ab139311f");
    }
}