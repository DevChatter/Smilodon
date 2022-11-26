using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class TagFollowEntityConfiguration : IEntityTypeConfiguration<TagFollow>
{
    public void Configure(EntityTypeBuilder<TagFollow> builder)
    {
        builder.ToTable("tag_follows");
        
        builder.HasKey(e => e.Id).HasName("tag_follows_pkey");

        builder.HasIndex(e => new { e.AccountId, e.TagId })
            .HasDatabaseName("index_tag_follows_on_account_id_and_tag_id")
            .IsUnique();

        builder.HasIndex(e => e.TagId).HasDatabaseName("index_tag_follows_on_tag_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.TagId).HasColumnName("tag_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.TagFollows)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_rails_091e831473");

        builder.HasOne(d => d.Tag)
            .WithMany(p => p.TagFollows)
            .HasForeignKey(d => d.TagId)
            .HasConstraintName("fk_rails_0deefe597f");
    }
}