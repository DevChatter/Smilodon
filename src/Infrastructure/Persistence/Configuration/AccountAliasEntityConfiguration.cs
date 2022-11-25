using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountAliasEntityConfiguration : IEntityTypeConfiguration<AccountAlias>
{
    public void Configure(EntityTypeBuilder<AccountAlias> builder)
    {
        builder.ToTable("account_aliases");

        builder.HasKey(e => e.Id).HasName("account_aliases_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_account_aliases_on_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Acct)
            .HasColumnType("character varying")
            .HasColumnName("acct")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Uri)
            .HasColumnType("character varying")
            .HasColumnName("uri")
            .HasDefaultValueSql("''::character varying");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AccountAliases)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_fc91575d08");
    }
}