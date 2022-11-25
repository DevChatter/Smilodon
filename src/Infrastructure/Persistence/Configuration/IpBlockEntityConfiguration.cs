using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class IpBlockEntityConfiguration : IEntityTypeConfiguration<IpBlock>
{
    public void Configure(EntityTypeBuilder<IpBlock> builder)
    {
        builder.ToTable("ip_blocks");

        builder.HasKey(e => e.Id).HasName("ip_blocks_pkey");

        // IPAddress does not implement IComparable, so we must do this manually in the migration
        // builder.HasIndex(e => e.Ip, "index_ip_blocks_on_ip")
        //     .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Comment)
            .HasColumnName("comment")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ExpiresAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("expires_at");

        builder.Property(e => e.Ip)
            .HasColumnName("ip")
            .HasDefaultValueSql("'0.0.0.0'::inet");

        builder.Property(e => e.Severity).HasColumnName("severity");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");
    }
}