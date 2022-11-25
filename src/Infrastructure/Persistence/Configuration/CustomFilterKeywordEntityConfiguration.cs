using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class CustomFilterKeywordEntityConfiguration : IEntityTypeConfiguration<CustomFilterKeyword>
{
    public void Configure(EntityTypeBuilder<CustomFilterKeyword> builder)
    {
        builder.ToTable("custom_filter_keywords");
        
        builder.HasKey(e => e.Id).HasName("custom_filter_keywords_pkey");

        builder.HasIndex(e => e.CustomFilterId)
            .HasDatabaseName("index_custom_filter_keywords_on_custom_filter_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.CustomFilterId).HasColumnName("custom_filter_id");

        builder.Property(e => e.Keyword)
            .HasColumnName("keyword")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.WholeWord)
            .IsRequired()
            .HasColumnName("whole_word")
            .HasDefaultValueSql("true");

        builder.HasOne(d => d.CustomFilter)
            .WithMany(p => p.CustomFilterKeywords)
            .HasForeignKey(d => d.CustomFilterId)
            .HasConstraintName("fk_rails_5a49a74012");
    }
}