using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class ArInternalMetadatumEntityConfiguration : IEntityTypeConfiguration<ArInternalMetadatum>
{
    public void Configure(EntityTypeBuilder<ArInternalMetadatum> builder)
    {
        builder.HasKey(e => e.Key)
            .HasName("ar_internal_metadata_pkey");

        builder.ToTable("ar_internal_metadata");

        builder.Property(e => e.Key)
            .HasColumnType("character varying")
            .HasColumnName("key");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Value)
            .HasColumnType("character varying")
            .HasColumnName("value");
    }
}