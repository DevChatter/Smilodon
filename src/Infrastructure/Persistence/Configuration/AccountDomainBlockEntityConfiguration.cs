using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountDomainBlockEntityConfiguration : IEntityTypeConfiguration<AccountDomainBlock>
{
    public void Configure(EntityTypeBuilder<AccountDomainBlock> builder)
    {
        builder.ToTable("account_domain_blocks");
        
        builder.HasKey(e => e.Id).HasName("account_domain_blocks_pkey");

        builder.HasIndex(e => new { e.AccountId, e.Domain })
            .HasDatabaseName("index_account_domain_blocks_on_account_id_and_domain")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Domain)
            .HasColumnType("character varying")
            .HasColumnName("domain");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AccountDomainBlocks)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_206c6029bd");
    }
}