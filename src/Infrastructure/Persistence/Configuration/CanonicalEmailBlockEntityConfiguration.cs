using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class CanonicalEmailBlockEntityConfiguration : IEntityTypeConfiguration<CanonicalEmailBlock>
{
    public void Configure(EntityTypeBuilder<CanonicalEmailBlock> builder)
    {
        builder.ToTable("canonical_email_blocks");
        
        builder.HasKey(e => e.Id).HasName("canonical_email_blocks_pkey");

        builder.HasIndex(e => e.CanonicalEmailHash)
            .HasDatabaseName("index_canonical_email_blocks_on_canonical_email_hash")
            .IsUnique();

        builder.HasIndex(e => e.ReferenceAccountId)
            .HasDatabaseName("index_canonical_email_blocks_on_reference_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CanonicalEmailHash)
            .HasColumnType("character varying")
            .HasColumnName("canonical_email_hash")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ReferenceAccountId).HasColumnName("reference_account_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.ReferenceAccount)
            .WithMany(p => p.CanonicalEmailBlocks)
            .HasForeignKey(d => d.ReferenceAccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_1ecb262096");
    }
}