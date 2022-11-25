using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class BookmarkEntityConfiguration : IEntityTypeConfiguration<Bookmark>
{
    public void Configure(EntityTypeBuilder<Bookmark> builder)
    {
        builder.ToTable("bookmarks");
        
        builder.HasKey(e => e.Id).HasName("bookmarks_pkey");

        builder.HasIndex(e => new { e.AccountId, e.StatusId })
            .HasDatabaseName("index_bookmarks_on_account_id_and_status_id")
            .IsUnique();

        builder.HasIndex(e => e.StatusId).HasDatabaseName("index_bookmarks_on_status_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.Bookmarks)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_rails_9f6ac182a6");

        builder.HasOne(d => d.Status)
            .WithMany(p => p.Bookmarks)
            .HasForeignKey(d => d.StatusId)
            .HasConstraintName("fk_rails_11207ffcfd");
    }
}