using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class OneTimeKeyEntityConfiguration : IEntityTypeConfiguration<OneTimeKey>
{
    public void Configure(EntityTypeBuilder<OneTimeKey> builder)
    {
        builder.ToTable("one_time_keys");
        
        builder.HasKey(e => e.Id).HasName("one_time_keys_pkey");

        builder.HasIndex(e => e.DeviceId).HasDatabaseName("index_one_time_keys_on_device_id");

        builder.HasIndex(e => e.KeyId).HasDatabaseName("index_one_time_keys_on_key_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.DeviceId).HasColumnName("device_id");

        builder.Property(e => e.Key)
            .HasColumnName("key")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.KeyId)
            .HasColumnType("character varying")
            .HasColumnName("key_id")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Signature)
            .HasColumnName("signature")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Device)
            .WithMany(p => p.OneTimeKeys)
            .HasForeignKey(d => d.DeviceId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_d3edd8c878");
    }
}