using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class PgheroSpaceStatEntityConfiguration : IEntityTypeConfiguration<PgheroSpaceStat>
{
    public void Configure(EntityTypeBuilder<PgheroSpaceStat> builder)
    {
        builder.ToTable("pghero_space_stats");
        
        builder.HasKey(e => e.Id).HasName("pghero_space_stats_pkey");

        builder.HasIndex(e => new { e.Database, e.CapturedAt })
            .HasDatabaseName("index_pghero_space_stats_on_database_and_captured_at");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CapturedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("captured_at");

        builder.Property(e => e.Database).HasColumnName("database");

        builder.Property(e => e.Relation).HasColumnName("relation");

        builder.Property(e => e.Schema).HasColumnName("schema");

        builder.Property(e => e.Size).HasColumnName("size");
    }
}