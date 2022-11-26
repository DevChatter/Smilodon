using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountTagEntityConfiguration : IEntityTypeConfiguration<AccountTag>
{
    public void Configure(EntityTypeBuilder<AccountTag> builder)
    {
        builder.HasNoKey();

        builder.ToTable("accounts_tags");

        builder.HasIndex(e => new { e.AccountId, e.TagId })
            .HasDatabaseName("index_accounts_tags_on_account_id_and_tag_id");

        builder.HasIndex(e => new { e.TagId, e.AccountId })
            .HasDatabaseName("index_accounts_tags_on_tag_id_and_account_id")
            .IsUnique();

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.TagId).HasColumnName("tag_id");
    }
}