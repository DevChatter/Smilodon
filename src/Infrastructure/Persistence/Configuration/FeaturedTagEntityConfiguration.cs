using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class FeaturedTagEntityConfiguration : IEntityTypeConfiguration<FeaturedTag>
{
    public void Configure(EntityTypeBuilder<FeaturedTag> builder)
    {
        builder.ToTable("featured_tags");
        
        builder.HasKey(e => e.Id).HasName("featured_tags_pkey");

        builder.HasIndex(e => new { e.AccountId, e.TagId })
            .HasDatabaseName("index_featured_tags_on_account_id_and_tag_id")
            .IsUnique();

        builder.HasIndex(e => e.TagId).HasDatabaseName("index_featured_tags_on_tag_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.LastStatusAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("last_status_at");

        builder.Property(e => e.Name)
            .HasColumnType("character varying")
            .HasColumnName("name");

        builder.Property(e => e.StatusesCount).HasColumnName("statuses_count");

        builder.Property(e => e.TagId).HasColumnName("tag_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.FeaturedTags)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_rails_174efcf15f");

        builder.HasOne(d => d.Tag)
            .WithMany(p => p.FeaturedTags)
            .HasForeignKey(d => d.TagId)
            .HasConstraintName("fk_rails_23a9055c7c");
    }
}