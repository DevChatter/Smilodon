using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountStatEntityConfiguration : IEntityTypeConfiguration<AccountStat>
{
    public void Configure(EntityTypeBuilder<AccountStat> builder)
    {
        builder.ToTable("account_stats");
        
        builder.HasKey(e => e.Id).HasName("account_stats_pkey");

        builder.HasIndex(e => e.AccountId)
            .HasDatabaseName("index_account_stats_on_account_id")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.FollowersCount).HasColumnName("followers_count");

        builder.Property(e => e.FollowingCount).HasColumnName("following_count");

        builder.Property(e => e.LastStatusAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("last_status_at");

        builder.Property(e => e.StatusesCount).HasColumnName("statuses_count");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithOne(p => p.AccountStat)
            .HasForeignKey<AccountStat>(d => d.AccountId)
            .HasConstraintName("fk_rails_215bb31ff1");
    }
}