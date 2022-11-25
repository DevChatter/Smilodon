using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountPinEntityConfiguration : IEntityTypeConfiguration<AccountPin>
{
    public void Configure(EntityTypeBuilder<AccountPin> builder)
    {
        builder.ToTable("account_pins");
        
        builder.HasKey(e => e.Id).HasName("account_pins_pkey");

        builder.HasIndex(e => new { e.AccountId, e.TargetAccountId })
            .HasDatabaseName("index_account_pins_on_account_id_and_target_account_id")
            .IsUnique();

        builder.HasIndex(e => e.TargetAccountId).HasDatabaseName("index_account_pins_on_target_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.TargetAccountId).HasColumnName("target_account_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AccountPinAccounts)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_d44979e5dd");

        builder.HasOne(d => d.TargetAccount)
            .WithMany(p => p.AccountPinTargetAccounts)
            .HasForeignKey(d => d.TargetAccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_a176e26c37");
    }
}