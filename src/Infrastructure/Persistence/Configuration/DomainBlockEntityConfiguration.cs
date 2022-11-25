using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class DomainBlockEntityConfiguration : IEntityTypeConfiguration<DomainBlock>
{
    public void Configure(EntityTypeBuilder<DomainBlock> builder)
    {
        builder.ToTable("domain_blocks");
        
        builder.HasKey(e => e.Id).HasName("domain_blocks_pkey");

        builder.HasIndex(e => e.Domain)
            .HasDatabaseName("index_domain_blocks_on_domain")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Domain)
            .HasColumnType("character varying")
            .HasColumnName("domain")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Obfuscate).HasColumnName("obfuscate");

        builder.Property(e => e.PrivateComment).HasColumnName("private_comment");

        builder.Property(e => e.PublicComment).HasColumnName("public_comment");

        builder.Property(e => e.RejectMedia).HasColumnName("reject_media");

        builder.Property(e => e.RejectReports).HasColumnName("reject_reports");

        builder.Property(e => e.Severity)
            .HasColumnName("severity")
            .HasDefaultValueSql("0");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");
    }
}