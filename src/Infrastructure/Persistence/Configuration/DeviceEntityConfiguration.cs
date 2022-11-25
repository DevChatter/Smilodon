using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class DeviceEntityConfiguration : IEntityTypeConfiguration<Device>
{
    public void Configure(EntityTypeBuilder<Device> builder)
    {
        builder.ToTable("devices");
        
        builder.HasKey(e => e.Id).HasName("devices_pkey");

        builder.HasIndex(e => e.AccessTokenId).HasDatabaseName("index_devices_on_access_token_id");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_devices_on_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccessTokenId).HasColumnName("access_token_id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.DeviceId)
            .HasColumnType("character varying")
            .HasColumnName("device_id")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.FingerprintKey)
            .HasColumnName("fingerprint_key")
            .HasDefaultValueSql("''::text");

        // builder.Property(e => e.IdbuilderKey)
        //     .HasColumnName("idbuilder_key")
        //     .HasDefaultValueSql("''::text");

        builder.Property(e => e.Name)
            .HasColumnType("character varying")
            .HasColumnName("name")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.AccessToken)
            .WithMany(p => p.Devices)
            .HasForeignKey(d => d.AccessTokenId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_393f74df68");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.Devices)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_a796b75798");
    }
}