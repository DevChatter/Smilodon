using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class StatusTrendEntityConfiguration : IEntityTypeConfiguration<StatusTrend>
{
    public void Configure(EntityTypeBuilder<StatusTrend> builder)
    {
        builder.ToTable("status_trends");
        
        builder.HasKey(e => e.Id).HasName("status_trends_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_status_trends_on_account_id");

        builder.HasIndex(e => e.StatusId)
            .HasDatabaseName("index_status_trends_on_status_id")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Allowed).HasColumnName("allowed");

        builder.Property(e => e.Language)
            .HasColumnType("character varying")
            .HasColumnName("language");

        builder.Property(e => e.Rank).HasColumnName("rank");

        builder.Property(e => e.Score).HasColumnName("score");

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.StatusTrends)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_rails_a6b527ea49");

        builder.HasOne(d => d.Status)
            .WithOne(p => p.StatusTrend)
            .HasForeignKey<StatusTrend>(d => d.StatusId)
            .HasConstraintName("fk_rails_68c610dc1a");
    }
}