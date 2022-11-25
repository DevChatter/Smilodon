using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class CustomFilterStatusEntityConfiguration : IEntityTypeConfiguration<CustomFilterStatus>
{
    public void Configure(EntityTypeBuilder<CustomFilterStatus> builder)
    {
        builder.ToTable("custom_filter_statuses");
        
        builder.HasKey(e => e.Id).HasName("custom_filter_statuses_pkey");

        builder.HasIndex(e => e.CustomFilterId)
            .HasDatabaseName("index_custom_filter_statuses_on_custom_filter_id");

        builder.HasIndex(e => e.StatusId)
            .HasDatabaseName("index_custom_filter_statuses_on_status_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.CustomFilterId).HasColumnName("custom_filter_id");

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.CustomFilter)
            .WithMany(p => p.CustomFilterStatuses)
            .HasForeignKey(d => d.CustomFilterId)
            .HasConstraintName("fk_rails_e2ddaf5b14");

        builder.HasOne(d => d.Status)
            .WithMany(p => p.CustomFilterStatuses)
            .HasForeignKey(d => d.StatusId)
            .HasConstraintName("fk_rails_2f6d20c0cf");
    }
}