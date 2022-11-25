using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class FollowRequestEntityConfiguration : IEntityTypeConfiguration<FollowRequest>
{
    public void Configure(EntityTypeBuilder<FollowRequest> builder)
    {
        builder.ToTable("follow_requests");
        
        builder.HasKey(e => e.Id).HasName("follow_requests_pkey");

        builder.HasIndex(e => new { e.AccountId, e.TargetAccountId })
            .HasDatabaseName("index_follow_requests_on_account_id_and_target_account_id")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Languages)
            .HasColumnType("character varying[]")
            .HasColumnName("languages");

        builder.Property(e => e.Notify).HasColumnName("notify");

        builder.Property(e => e.ShowReblogs)
            .IsRequired()
            .HasColumnName("show_reblogs")
            .HasDefaultValueSql("true");

        builder.Property(e => e.TargetAccountId).HasColumnName("target_account_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Uri)
            .HasColumnType("character varying")
            .HasColumnName("uri");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.FollowRequestAccounts)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_76d644b0e7");

        builder.HasOne(d => d.TargetAccount)
            .WithMany(p => p.FollowRequestTargetAccounts)
            .HasForeignKey(d => d.TargetAccountId)
            .HasConstraintName("fk_9291ec025d");
    }
}