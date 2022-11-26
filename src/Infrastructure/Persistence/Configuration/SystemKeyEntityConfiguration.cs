using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class SystemKeyEntityConfiguration : IEntityTypeConfiguration<SystemKey>
{
    public void Configure(EntityTypeBuilder<SystemKey> builder)
    {
        builder.ToTable("system_keys");
        
        builder.HasKey(e => e.Id).HasName("system_keys_pkey");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Key).HasColumnName("key");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");
    }
}