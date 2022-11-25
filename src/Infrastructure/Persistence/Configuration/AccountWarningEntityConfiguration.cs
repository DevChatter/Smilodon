using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountWarningEntityConfiguration : IEntityTypeConfiguration<AccountWarning>
{
    public void Configure(EntityTypeBuilder<AccountWarning> builder)
    {
        builder.ToTable("account_warnings");

        builder.HasKey(e => e.Id).HasName("account_warnings_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_account_warnings_on_account_id");

        builder.HasIndex(e => e.TargetAccountId).HasDatabaseName("index_account_warnings_on_target_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Action).HasColumnName("action");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.OverruledAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("overruled_at");

        builder.Property(e => e.ReportId).HasColumnName("report_id");

        builder.Property(e => e.StatusIds)
            .HasColumnType("character varying[]")
            .HasColumnName("status_ids");

        builder.Property(e => e.TargetAccountId).HasColumnName("target_account_id");

        builder.Property(e => e.Text)
            .HasColumnName("text")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AccountWarningAccounts)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_a65a1bf71b");

        builder.HasOne(d => d.Report)
            .WithMany(p => p.AccountWarnings)
            .HasForeignKey(d => d.ReportId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_8f2bab4b16");

        builder.HasOne(d => d.TargetAccount)
            .WithMany(p => p.AccountWarningTargetAccounts)
            .HasForeignKey(d => d.TargetAccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_a7ebbb1e37");
    }
}