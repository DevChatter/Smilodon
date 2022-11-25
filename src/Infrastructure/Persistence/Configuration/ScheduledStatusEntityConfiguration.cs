using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class ScheduledStatusEntityConfiguration : IEntityTypeConfiguration<ScheduledStatus>
{
    public void Configure(EntityTypeBuilder<ScheduledStatus> builder)
    {
        builder.ToTable("scheduled_statuses");
        
        builder.HasKey(e => e.Id).HasName("scheduled_statuses_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_scheduled_statuses_on_account_id");

        builder.HasIndex(e => e.ScheduledAt).HasDatabaseName("index_scheduled_statuses_on_scheduled_at");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Params)
            .HasColumnType("jsonb")
            .HasColumnName("params");

        builder.Property(e => e.ScheduledAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("scheduled_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.ScheduledStatuses)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_23bd9018f9");
    }
}