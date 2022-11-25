using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class AdminActionLogEntityConfiguration : IEntityTypeConfiguration<AdminActionLog>
{
    public void Configure(EntityTypeBuilder<AdminActionLog> builder)
    {
        builder.ToTable("admin_action_logs");
        
        builder.HasKey(e => e.Id).HasName("admin_action_logs_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_admin_action_logs_on_account_id");

        builder.HasIndex(e => new { e.TargetType, e.TargetId })
            .HasDatabaseName("index_admin_action_logs_on_target_type_and_target_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Action)
            .HasColumnType("character varying")
            .HasColumnName("action")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.HumanIdentifier)
            .HasColumnType("character varying")
            .HasColumnName("human_identifier");

        builder.Property(e => e.Permalink)
            .HasColumnType("character varying")
            .HasColumnName("permalink");

        builder.Property(e => e.RouteParam)
            .HasColumnType("character varying")
            .HasColumnName("route_param");

        builder.Property(e => e.TargetId).HasColumnName("target_id");

        builder.Property(e => e.TargetType)
            .HasColumnType("character varying")
            .HasColumnName("target_type");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.AdminActionLogs)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_a7667297fa");
    }
}