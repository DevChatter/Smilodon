using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class PollEntityConfiguration : IEntityTypeConfiguration<Poll>
{
    public void Configure(EntityTypeBuilder<Poll> builder)
    {
        builder.ToTable("polls");
        
        builder.HasKey(e => e.Id).HasName("polls_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_polls_on_account_id");

        builder.HasIndex(e => e.StatusId).HasDatabaseName("index_polls_on_status_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CachedTallies)
            .HasColumnName("cached_tallies")
            .HasDefaultValueSql("'{}'::bigint[]");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ExpiresAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("expires_at");

        builder.Property(e => e.HideTotals).HasColumnName("hide_totals");

        builder.Property(e => e.LastFetchedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("last_fetched_at");

        builder.Property(e => e.LockVersion).HasColumnName("lock_version");

        builder.Property(e => e.Multiple).HasColumnName("multiple");

        builder.Property(e => e.Options)
            .HasColumnType("character varying[]")
            .HasColumnName("options")
            .HasDefaultValueSql("'{}'::character varying[]");

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.VotersCount).HasColumnName("voters_count");

        builder.Property(e => e.VotesCount).HasColumnName("votes_count");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.Polls)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_5b19a0c011");

        builder.HasOne(d => d.Status)
            .WithMany(p => p.Polls)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_3e0d9f1115");
    }
}