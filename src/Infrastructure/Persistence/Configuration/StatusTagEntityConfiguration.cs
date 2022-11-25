using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class StatusTagEntityConfiguration : IEntityTypeConfiguration<StatusTag>
{
    public void Configure(EntityTypeBuilder<StatusTag> builder)
    {
        builder.HasNoKey();

        builder.ToTable("statuses_tags");

        builder.HasIndex(e => e.StatusId).HasDatabaseName("index_statuses_tags_on_status_id");

        builder.HasIndex(e => new { e.TagId, e.StatusId })
            .HasDatabaseName("index_statuses_tags_on_tag_id_and_status_id")
            .IsUnique();

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.Property(e => e.TagId).HasColumnName("tag_id");

        builder.HasOne(d => d.Status)
            .WithMany()
            .HasForeignKey(d => d.StatusId)
            .HasConstraintName("fk_rails_df0fe11427");

        builder.HasOne(d => d.Tag)
            .WithMany()
            .HasForeignKey(d => d.TagId)
            .HasConstraintName("fk_3081861e21");
    }
}