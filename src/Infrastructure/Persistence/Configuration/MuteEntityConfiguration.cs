using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class MuteEntityConfiguration : IEntityTypeConfiguration<Mute>
{
    public void Configure(EntityTypeBuilder<Mute> builder)
    {
        builder.ToTable("mutes");
        
        builder.HasKey(e => e.Id).HasName("mutes_pkey");

        builder.HasIndex(e => new { e.AccountId, e.TargetAccountId })
            .HasDatabaseName("index_mutes_on_account_id_and_target_account_id")
            .IsUnique();

        builder.HasIndex(e => e.TargetAccountId).HasDatabaseName("index_mutes_on_target_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ExpiresAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("expires_at");

        builder.Property(e => e.HideNotifications)
            .IsRequired()
            .HasColumnName("hide_notifications")
            .HasDefaultValueSql("true");

        builder.Property(e => e.TargetAccountId).HasColumnName("target_account_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.MuteAccounts)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_b8d8daf315");

        builder.HasOne(d => d.TargetAccount)
            .WithMany(p => p.MuteTargetAccounts)
            .HasForeignKey(d => d.TargetAccountId)
            .HasConstraintName("fk_eecff219ea");
    }
}