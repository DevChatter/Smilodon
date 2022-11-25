using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AnnouncementEntityConfiguration : IEntityTypeConfiguration<Announcement>
{
    public void Configure(EntityTypeBuilder<Announcement> builder)
    {
        builder.ToTable("announcements");
        
        builder.HasKey(e => e.Id).HasName("announcements_pkey");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AllDay).HasColumnName("all_day");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.EndsAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("ends_at");

        builder.Property(e => e.Published).HasColumnName("published");

        builder.Property(e => e.PublishedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("published_at");

        builder.Property(e => e.ScheduledAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("scheduled_at");

        builder.Property(e => e.StartsAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("starts_at");

        builder.Property(e => e.StatusIds).HasColumnName("status_ids");

        builder.Property(e => e.Text)
            .HasColumnName("text")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");
    }
}