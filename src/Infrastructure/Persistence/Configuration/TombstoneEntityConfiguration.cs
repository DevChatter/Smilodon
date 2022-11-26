using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class TombstoneEntityConfiguration : IEntityTypeConfiguration<Tombstone>
{
    public void Configure(EntityTypeBuilder<Tombstone> builder)
    {
        builder.ToTable("tombstones");
        
        builder.HasKey(e => e.Id).HasName("tombstones_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_tombstones_on_account_id");

        builder.HasIndex(e => e.Uri).HasDatabaseName("index_tombstones_on_uri");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.ByModerator).HasColumnName("by_moderator");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Uri)
            .HasColumnType("character varying")
            .HasColumnName("uri");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.Tombstones)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_f95b861449");
    }
}