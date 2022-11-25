using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class InstanceEntityConfiguration : IEntityTypeConfiguration<Instance>
{
    public void Configure(EntityTypeBuilder<Instance> builder)
    {
        builder.HasNoKey();

        builder.ToView("instances");

        builder.Property(e => e.AccountsCount).HasColumnName("accounts_count");

        builder.Property(e => e.Domain)
            .HasColumnType("character varying")
            .HasColumnName("domain");
    }
}