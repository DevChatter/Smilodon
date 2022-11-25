using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountStatusesCleanupPolicyEntityConfiguration : IEntityTypeConfiguration<AccountStatusesCleanupPolicy>
{
    public void Configure(EntityTypeBuilder<AccountStatusesCleanupPolicy> builder)
    {
        builder.ToTable("account_statuses_cleanup_policies");
        
        builder.HasKey(e => e.Id).HasName("account_statuses_cleanup_policies_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_account_statuses_cleanup_policies_on_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Enabled)
            .IsRequired()
            .HasColumnName("enabled")
            .HasDefaultValueSql("true");

        builder.Property(e => e.KeepDirect)
            .IsRequired()
            .HasColumnName("keep_direct")
            .HasDefaultValueSql("true");

        builder.Property(e => e.KeepMedia).HasColumnName("keep_media");

        builder.Property(e => e.KeepPinned)
            .IsRequired()
            .HasColumnName("keep_pinned")
            .HasDefaultValueSql("true");

        builder.Property(e => e.KeepPolls).HasColumnName("keep_polls");

        builder.Property(e => e.KeepSelfBookmark)
            .IsRequired()
            .HasColumnName("keep_self_bookmark")
            .HasDefaultValueSql("true");

        builder.Property(e => e.KeepSelfFav)
            .IsRequired()
            .HasColumnName("keep_self_fav")
            .HasDefaultValueSql("true");

        builder.Property(e => e.MinFavs).HasColumnName("min_favs");

        builder.Property(e => e.MinReblogs).HasColumnName("min_reblogs");

        builder.Property(e => e.MinStatusAge)
            .HasColumnName("min_status_age")
            .HasDefaultValueSql("1209600");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AccountStatusesCleanupPolicies)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_rails_23d5f73cfe");
    }
}