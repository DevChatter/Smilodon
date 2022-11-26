using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class PreviewCardTrendEntityConfiguration : IEntityTypeConfiguration<PreviewCardTrend>
{
    public void Configure(EntityTypeBuilder<PreviewCardTrend> builder)
    {
        builder.ToTable("preview_card_trends");
        
        builder.HasKey(e => e.Id).HasName("preview_card_trends_pkey");

        builder.HasIndex(e => e.PreviewCardId)
            .HasDatabaseName("index_preview_card_trends_on_preview_card_id")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Allowed).HasColumnName("allowed");

        builder.Property(e => e.Language)
            .HasColumnType("character varying")
            .HasColumnName("language");

        builder.Property(e => e.PreviewCardId).HasColumnName("preview_card_id");

        builder.Property(e => e.Rank).HasColumnName("rank");

        builder.Property(e => e.Score).HasColumnName("score");

        builder.HasOne(d => d.PreviewCard)
            .WithOne(p => p.PreviewCardTrend)
            .HasForeignKey<PreviewCardTrend>(d => d.PreviewCardId)
            .HasConstraintName("fk_rails_371593db34");
    }
}