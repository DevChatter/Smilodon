using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class FavouriteEntityConfiguration : IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> builder)
    {
        builder.ToTable("favourites");
        
        builder.HasKey(e => e.Id).HasName("favourites_pkey");

        builder.HasIndex(e => new { e.AccountId, e.Id })
            .HasDatabaseName("index_favourites_on_account_id_and_id");

        builder.HasIndex(e => new { e.AccountId, e.StatusId })
            .HasDatabaseName("index_favourites_on_account_id_and_status_id")
            .IsUnique();

        builder.HasIndex(e => e.StatusId).HasDatabaseName("index_favourites_on_status_id");

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
            .WithMany(p => p.Favourites)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_5eb6c2b873");

        builder.HasOne(d => d.Status)
            .WithMany(p => p.Favourites)
            .HasForeignKey(d => d.StatusId)
            .HasConstraintName("fk_b0e856845e");
    }
}