using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class EmailDomainBlockEntityConfiguration : IEntityTypeConfiguration<EmailDomainBlock>
{
    public void Configure(EntityTypeBuilder<EmailDomainBlock> builder)
    {
        builder.ToTable("email_domain_blocks");
        
        builder.HasKey(e => e.Id).HasName("email_domain_blocks_pkey");

        builder.HasIndex(e => e.Domain)
            .HasDatabaseName("index_email_domain_blocks_on_domain")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Domain)
            .HasColumnType("character varying")
            .HasColumnName("domain")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.ParentId).HasColumnName("parent_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Parent)
            .WithMany(p => p.InverseParent)
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_408efe0a15");
    }
}