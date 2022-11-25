using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class MarkerEntityConfiguration : IEntityTypeConfiguration<Marker>
{
    public void Configure(EntityTypeBuilder<Marker> builder)
    {
        builder.ToTable("markers");
        
        builder.HasKey(e => e.Id).HasName("markers_pkey");

        builder.HasIndex(e => new { e.UserId, e.Timeline })
            .HasDatabaseName("index_markers_on_user_id_and_timeline")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.LastReadId).HasColumnName("last_read_id");

        builder.Property(e => e.LockVersion).HasColumnName("lock_version");

        builder.Property(e => e.Timeline)
            .HasColumnType("character varying")
            .HasColumnName("timeline")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.User)
            .WithMany(p => p.Markers)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_a7009bc2b6");
    }
}