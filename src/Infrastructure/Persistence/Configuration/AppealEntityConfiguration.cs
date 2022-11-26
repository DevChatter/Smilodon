using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AppealEntityConfiguration : IEntityTypeConfiguration<Appeal>
{
    public void Configure(EntityTypeBuilder<Appeal> builder)
    {
        builder.ToTable("appeals");
        
        builder.HasKey(e => e.Id).HasName("appeals_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_appeals_on_account_id");

        builder.HasIndex(e => e.AccountWarningId)
            .HasDatabaseName("index_appeals_on_account_warning_id")
            .IsUnique();

        builder.HasIndex(e => e.ApprovedByAccountId)
            .HasDatabaseName("index_appeals_on_approved_by_account_id")
            .HasFilter("(approved_by_account_id IS NOT NULL)");

        builder.HasIndex(e => e.RejectedByAccountId)
            .HasDatabaseName("index_appeals_on_rejected_by_account_id")
            .HasFilter("(rejected_by_account_id IS NOT NULL)");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.AccountWarningId).HasColumnName("account_warning_id");

        builder.Property(e => e.ApprovedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("approved_at");

        builder.Property(e => e.ApprovedByAccountId).HasColumnName("approved_by_account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.RejectedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("rejected_at");

        builder.Property(e => e.RejectedByAccountId).HasColumnName("rejected_by_account_id");

        builder.Property(e => e.Text)
            .HasColumnName("text")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AppealAccounts)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_rails_ea84881569");

        builder.HasOne(d => d.AccountWarning)
            .WithOne(p => p.Appeal)
            .HasForeignKey<Appeal>(d => d.AccountWarningId)
            .HasConstraintName("fk_rails_a99f14546e");

        builder.HasOne(d => d.ApprovedByAccount)
            .WithMany(p => p.AppealApprovedByAccounts)
            .HasForeignKey(d => d.ApprovedByAccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_9deb2f63ad");

        builder.HasOne(d => d.RejectedByAccount)
            .WithMany(p => p.AppealRejectedByAccounts)
            .HasForeignKey(d => d.RejectedByAccountId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_501c3a6e13");
    }
}