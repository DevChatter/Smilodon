using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class IdbuilderEntityConfiguration : IEntityTypeConfiguration<Identity>
{
    public void Configure(EntityTypeBuilder<Identity> builder)
    {
        builder.ToTable("identities");
        
        builder.HasKey(e => e.Id).HasName("identities_pkey");

        builder.HasIndex(e => e.UserId).HasDatabaseName("index_identities_on_user_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Provider)
            .HasColumnType("character varying")
            .HasColumnName("provider")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Uid)
            .HasColumnType("character varying")
            .HasColumnName("uid")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.User)
            .WithMany(p => p.Identities)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_bea040f377");
    }
}