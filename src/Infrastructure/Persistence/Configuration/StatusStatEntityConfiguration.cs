using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class StatusStatEntityConfiguration : IEntityTypeConfiguration<StatusStat>
{
    public void Configure(EntityTypeBuilder<StatusStat> builder)
    {
        builder.ToTable("status_stats");
        
        builder.HasKey(e => e.Id).HasName("status_stats_pkey");

        builder.HasIndex(e => e.StatusId)
            .HasDatabaseName("index_status_stats_on_status_id")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.FavouritesCount).HasColumnName("favourites_count");

        builder.Property(e => e.ReblogsCount).HasColumnName("reblogs_count");

        builder.Property(e => e.RepliesCount).HasColumnName("replies_count");

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Status)
            .WithOne(p => p.StatusStat)
            .HasForeignKey<StatusStat>(d => d.StatusId)
            .HasConstraintName("fk_rails_4a247aac42");
    }
}