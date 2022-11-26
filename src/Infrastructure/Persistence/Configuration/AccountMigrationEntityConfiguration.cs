using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountMigrationEntityConfiguration : IEntityTypeConfiguration<AccountMigration>
{
    public void Configure(EntityTypeBuilder<AccountMigration> builder)
    {
        builder.ToTable("account_migrations");
        
        builder.HasKey(e => e.Id).HasName("account_migrations_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_account_migrations_on_account_id");

        builder.HasIndex(e => e.TargetAccountId)
            .HasDatabaseName("index_account_migrations_on_target_account_id")
            .HasFilter("(target_account_id IS NOT NULL)");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Acct)
            .HasColumnType("character varying")
            .HasColumnName("acct")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.FollowersCount).HasColumnName("followers_count");

        builder.Property(e => e.TargetAccountId).HasColumnName("target_account_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AccountMigrationAccounts)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_c9f701caaf");

        builder.HasOne(d => d.TargetAccount)
            .WithMany(p => p.AccountMigrationTargetAccounts)
            .HasForeignKey(d => d.TargetAccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_d9a8dad070");
    }
}