using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountModerationNoteEntityConfiguration : IEntityTypeConfiguration<AccountModerationNote>
{
    public void Configure(EntityTypeBuilder<AccountModerationNote> builder)
    {
        builder.ToTable("account_moderation_notes");
        
        builder.HasKey(e => e.Id).HasName("account_moderation_notes_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_account_moderation_notes_on_account_id");

        builder.HasIndex(e => e.TargetAccountId).HasDatabaseName("index_account_moderation_notes_on_target_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Content).HasColumnName("content");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.TargetAccountId).HasColumnName("target_account_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AccountModerationNoteAccounts)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_rails_3f8b75089b");

        builder.HasOne(d => d.TargetAccount)
            .WithMany(p => p.AccountModerationNoteTargetAccounts)
            .HasForeignKey(d => d.TargetAccountId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("fk_rails_dd62ed5ac3");
    }
}