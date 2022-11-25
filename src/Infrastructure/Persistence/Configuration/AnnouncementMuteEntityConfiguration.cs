using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AnnouncementMuteEntityConfiguration : IEntityTypeConfiguration<AnnouncementMute>
{
    public void Configure(EntityTypeBuilder<AnnouncementMute> builder)
    {
        builder.ToTable("announcement_mutes");
        
        builder.HasKey(e => e.Id).HasName("announcement_mutes_pkey");

        builder.HasIndex(e => new { e.AccountId, e.AnnouncementId })
            .HasDatabaseName("index_announcement_mutes_on_account_id_and_announcement_id")
            .IsUnique();

        builder.HasIndex(e => e.AnnouncementId)
            .HasDatabaseName("index_announcement_mutes_on_announcement_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.AnnouncementId).HasColumnName("announcement_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AnnouncementMutes)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_9c99f8e835");

        builder.HasOne(d => d.Announcement)
            .WithMany(p => p.AnnouncementMutes)
            .HasForeignKey(d => d.AnnouncementId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_e35401adf1");
    }
}