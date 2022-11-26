using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class ReportEntityConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ToTable("reports");
        
        builder.HasKey(e => e.Id).HasName("reports_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_reports_on_account_id");

        builder.HasIndex(e => e.ActionTakenByAccountId)
            .HasDatabaseName("index_reports_on_action_taken_by_account_id")
            .HasFilter("(action_taken_by_account_id IS NOT NULL)");

        builder.HasIndex(e => e.AssignedAccountId)
            .HasDatabaseName("index_reports_on_assigned_account_id")
            .HasFilter("(assigned_account_id IS NOT NULL)");

        builder.HasIndex(e => e.TargetAccountId).HasDatabaseName("index_reports_on_target_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.ActionTakenAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("action_taken_at");

        builder.Property(e => e.ActionTakenByAccountId).HasColumnName("action_taken_by_account_id");

        builder.Property(e => e.AssignedAccountId).HasColumnName("assigned_account_id");

        builder.Property(e => e.Category).HasColumnName("category");

        builder.Property(e => e.Comment)
            .HasColumnName("comment")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Forwarded).HasColumnName("forwarded");

        builder.Property(e => e.RuleIds).HasColumnName("rule_ids");

        builder.Property(e => e.StatusIds)
            .HasColumnName("status_ids")
            .HasDefaultValueSql("'{}'::bigint[]");

        builder.Property(e => e.TargetAccountId).HasColumnName("target_account_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Uri)
            .HasColumnType("character varying")
            .HasColumnName("uri");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.ReportAccounts)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_4b81f7522c");

        builder.HasOne(d => d.ActionTakenByAccount)
            .WithMany(p => p.ReportActionTakenByAccounts)
            .HasForeignKey(d => d.ActionTakenByAccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_bca45b75fd");

        builder.HasOne(d => d.AssignedAccount)
            .WithMany(p => p.ReportAssignedAccounts)
            .HasForeignKey(d => d.AssignedAccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_4e7a498fb4");

        builder.HasOne(d => d.TargetAccount)
            .WithMany(p => p.ReportTargetAccounts)
            .HasForeignKey(d => d.TargetAccountId)
            .HasConstraintName("fk_eb37af34f0");
    }
}