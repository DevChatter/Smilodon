using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class CustomFilterEntityConfiguration : IEntityTypeConfiguration<CustomFilter>
{
    public void Configure(EntityTypeBuilder<CustomFilter> builder)
    {
        builder.ToTable("custom_filters");
        
        builder.HasKey(e => e.Id).HasName("custom_filters_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_custom_filters_on_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Action).HasColumnName("action");

        builder.Property(e => e.Context)
            .HasColumnType("character varying[]")
            .HasColumnName("context")
            .HasDefaultValueSql("'{}'::character varying[]");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ExpiresAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("expires_at");

        builder.Property(e => e.Phrase)
            .HasColumnName("phrase")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.CustomFilters)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_8b8d786993");
    }
}