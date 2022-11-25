using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountNoteEntityConfiguration : IEntityTypeConfiguration<AccountNote>
{
    public void Configure(EntityTypeBuilder<AccountNote> builder)
    {
        builder.ToTable("account_notes");
        
        builder.HasKey(e => e.Id).HasName("account_notes_pkey");

        builder.HasIndex(e => new { e.AccountId, e.TargetAccountId })
            .HasDatabaseName("index_account_notes_on_account_id_and_target_account_id")
            .IsUnique();

        builder.HasIndex(e => e.TargetAccountId).HasDatabaseName("index_account_notes_on_target_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Comment).HasColumnName("comment");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.TargetAccountId).HasColumnName("target_account_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AccountNoteAccounts)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_4ee4503c69");

        builder.HasOne(d => d.TargetAccount)
            .WithMany(p => p.AccountNoteTargetAccounts)
            .HasForeignKey(d => d.TargetAccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_2801b48f1a");
    }
}