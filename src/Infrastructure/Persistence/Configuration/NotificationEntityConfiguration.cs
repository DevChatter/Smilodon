using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class NotificationEntityConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        
        builder.HasKey(e => e.Id).HasName("notifications_pkey");

        builder.HasIndex(e => new { e.AccountId, e.Id, e.Type })
            .HasDatabaseName("index_notifications_on_account_id_and_id_and_type")
            .IsDescending(false, true, false);

        builder.HasIndex(e => new { e.ActivityId, e.ActivityType })
            .HasDatabaseName("index_notifications_on_activity_id_and_activity_type");

        builder.HasIndex(e => e.FromAccountId).HasDatabaseName("index_notifications_on_from_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.ActivityId).HasColumnName("activity_id");

        builder.Property(e => e.ActivityType)
            .HasColumnType("character varying")
            .HasColumnName("activity_type");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.FromAccountId).HasColumnName("from_account_id");

        builder.Property(e => e.Type)
            .HasColumnType("character varying")
            .HasColumnName("type");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.NotificationAccounts)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_c141c8ee55");

        builder.HasOne(d => d.FromAccount)
            .WithMany(p => p.NotificationFromAccounts)
            .HasForeignKey(d => d.FromAccountId)
            .HasConstraintName("fk_fbd6b0bf9e");
    }
}