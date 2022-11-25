using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class InviteEntityConfiguration : IEntityTypeConfiguration<Invite>
{
    public void Configure(EntityTypeBuilder<Invite> builder)
    {
        builder.ToTable("invites");
        
        builder.HasKey(e => e.Id).HasName("invites_pkey");

        builder.HasIndex(e => e.Code)
            .HasDatabaseName("index_invites_on_code")
            .IsUnique();

        builder.HasIndex(e => e.UserId).HasDatabaseName("index_invites_on_user_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Autofollow).HasColumnName("autofollow");

        builder.Property(e => e.Code)
            .HasColumnType("character varying")
            .HasColumnName("code")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Comment).HasColumnName("comment");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ExpiresAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("expires_at");

        builder.Property(e => e.MaxUses).HasColumnName("max_uses");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.Property(e => e.Uses).HasColumnName("uses");

        builder.HasOne(d => d.User)
            .WithMany(p => p.Invites)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_rails_ff69dbb2ac");
    }
}