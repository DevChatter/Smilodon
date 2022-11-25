using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class WebhookEntityConfiguration : IEntityTypeConfiguration<Webhook>
{
    public void Configure(EntityTypeBuilder<Webhook> builder)
    {
        builder.ToTable("webhooks");
        
        builder.HasKey(e => e.Id).HasName("webhooks_pkey");

        builder.HasIndex(e => e.Url)
            .HasDatabaseName("index_webhooks_on_url")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Enabled)
            .IsRequired()
            .HasColumnName("enabled")
            .HasDefaultValueSql("true");

        builder.Property(e => e.Events)
            .HasColumnType("character varying[]")
            .HasColumnName("events")
            .HasDefaultValueSql("'{}'::character varying[]");

        builder.Property(e => e.Secret)
            .HasColumnType("character varying")
            .HasColumnName("secret")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Url)
            .HasColumnType("character varying")
            .HasColumnName("url");
    }
}