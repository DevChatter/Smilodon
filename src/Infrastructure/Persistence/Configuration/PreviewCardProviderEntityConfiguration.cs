using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class PreviewCardProviderEntityConfiguration : IEntityTypeConfiguration<PreviewCardProvider>
{
    public void Configure(EntityTypeBuilder<PreviewCardProvider> builder)
    {
        builder.ToTable("preview_card_providers");
        
        builder.HasKey(e => e.Id).HasName("preview_card_providers_pkey");

        builder.HasIndex(e => e.Domain)
            .HasDatabaseName("index_preview_card_providers_on_domain")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Domain)
            .HasColumnType("character varying")
            .HasColumnName("domain")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.IconContentType)
            .HasColumnType("character varying")
            .HasColumnName("icon_content_type");

        builder.Property(e => e.IconFileName)
            .HasColumnType("character varying")
            .HasColumnName("icon_file_name");

        builder.Property(e => e.IconFileSize).HasColumnName("icon_file_size");

        builder.Property(e => e.IconUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("icon_updated_at");

        builder.Property(e => e.RequestedReviewAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("requested_review_at");

        builder.Property(e => e.ReviewedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("reviewed_at");

        builder.Property(e => e.Trendable).HasColumnName("trendable");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");
    }
}