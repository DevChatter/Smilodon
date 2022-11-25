using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class FollowEntityConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("follows");
        
        builder.HasKey(e => e.Id).HasName("follows_pkey");

        builder.HasIndex(e => new { e.AccountId, e.TargetAccountId })
            .HasDatabaseName("index_follows_on_account_id_and_target_account_id")
            .IsUnique();

        builder.HasIndex(e => e.TargetAccountId, "index_follows_on_target_account_id");

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
            .WithMany(p => p.FollowAccounts)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_32ed1b5560");

        builder.HasOne(d => d.TargetAccount)
            .WithMany(p => p.FollowTargetAccounts)
            .HasForeignKey(d => d.TargetAccountId)
            .HasConstraintName("fk_745ca29eac");
    }
}