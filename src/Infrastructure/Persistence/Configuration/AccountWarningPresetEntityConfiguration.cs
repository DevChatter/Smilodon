using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AccountWarningPresetEntityConfiguration : IEntityTypeConfiguration<AccountWarningPreset>
{
    public void Configure(EntityTypeBuilder<AccountWarningPreset> builder)
    {
        builder.ToTable("account_warning_presets");
        
        builder.HasKey(e => e.Id).HasName("account_warning_presets_pkey");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Text)
            .HasColumnName("text")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.Title)
            .HasColumnType("character varying")
            .HasColumnName("title")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");
    }
}