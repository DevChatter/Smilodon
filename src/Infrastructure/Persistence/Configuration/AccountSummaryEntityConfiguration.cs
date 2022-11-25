using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountSummaryEntityConfiguration : IEntityTypeConfiguration<AccountSummary>
{
    public void Configure(EntityTypeBuilder<AccountSummary> builder)
    {
        builder.HasNoKey();

        builder.ToView("account_summaries");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Language)
            .HasColumnType("character varying")
            .HasColumnName("language");

        builder.Property(e => e.Sensitive).HasColumnName("sensitive");
    }
}