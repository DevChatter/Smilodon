using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class TagEntityConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        
        builder.HasKey(e => e.Id).HasName("tags_pkey");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.DisplayName)
            .HasColumnType("character varying")
            .HasColumnName("display_name");

        builder.Property(e => e.LastStatusAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("last_status_at");

        builder.Property(e => e.Listable).HasColumnName("listable");

        builder.Property(e => e.MaxScore).HasColumnName("max_score");

        builder.Property(e => e.MaxScoreAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("max_score_at");

        builder.Property(e => e.Name)
            .HasColumnType("character varying")
            .HasColumnName("name")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.RequestedReviewAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("requested_review_at");

        builder.Property(e => e.ReviewedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("reviewed_at");

        builder.Property(e => e.Trendable).HasColumnName("trendable");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Usable).HasColumnName("usable");
    }
}