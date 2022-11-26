using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class StatusPinEntityConfiguration : IEntityTypeConfiguration<StatusPin>
{
    public void Configure(EntityTypeBuilder<StatusPin> builder)
    {
        builder.ToTable("status_pins");
        
        builder.HasKey(e => e.Id).HasName("status_pins_pkey");

        builder.HasIndex(e => new { e.AccountId, e.StatusId })
            .HasDatabaseName("index_status_pins_on_account_id_and_status_id")
            .IsUnique();

        builder.HasIndex(e => e.StatusId).HasDatabaseName("index_status_pins_on_status_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.StatusPins)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_d4cb435b62");

        builder.HasOne(d => d.Status)
            .WithMany(p => p.StatusPins)
            .HasForeignKey(d => d.StatusId)
            .HasConstraintName("fk_rails_65c05552f1");
    }
}