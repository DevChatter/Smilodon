using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class UnavailableDomainEntityConfiguration : IEntityTypeConfiguration<UnavailableDomain>
{
    public void Configure(EntityTypeBuilder<UnavailableDomain> builder)
    {
        builder.ToTable("unavailable_domains");
        
        builder.HasKey(e => e.Id).HasName("unavailable_domains_pkey");

        builder.HasIndex(e => e.Domain)
            .HasDatabaseName("index_unavailable_domains_on_domain")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Domain)
            .HasColumnType("character varying")
            .HasColumnName("domain")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");
    }
}