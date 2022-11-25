using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class CustomEmojiCategoryEntityConfiguration : IEntityTypeConfiguration<CustomEmojiCategory>
{
    public void Configure(EntityTypeBuilder<CustomEmojiCategory> builder)
    {
        builder.ToTable("custom_emoji_categories");
        
        builder.HasKey(e => e.Id).HasName("custom_emoji_categories_pkey");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("index_custom_emoji_categories_on_name")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Name)
            .HasColumnType("character varying")
            .HasColumnName("name");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");
    }
}