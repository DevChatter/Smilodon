using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class OAuthApplicationEntityConfiguration : IEntityTypeConfiguration<OAuthApplication>
{
    public void Configure(EntityTypeBuilder<OAuthApplication> builder)
    {
        builder.ToTable("oauth_applications");
        
        builder.HasKey(e => e.Id).HasName("oauth_applications_pkey");

        builder.HasIndex(e => new { e.OwnerId, e.OwnerType })
            .HasDatabaseName("index_oauth_applications_on_owner_id_and_owner_type");

        builder.HasIndex(e => e.Uid)
            .HasDatabaseName("index_oauth_applications_on_uid")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Confidential)
            .IsRequired()
            .HasColumnName("confidential")
            .HasDefaultValueSql("true");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Name)
            .HasColumnType("character varying")
            .HasColumnName("name");

        builder.Property(e => e.OwnerId).HasColumnName("owner_id");

        builder.Property(e => e.OwnerType)
            .HasColumnType("character varying")
            .HasColumnName("owner_type");

        builder.Property(e => e.RedirectUri).HasColumnName("redirect_uri");

        builder.Property(e => e.Scopes)
            .HasColumnType("character varying")
            .HasColumnName("scopes")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Secret)
            .HasColumnType("character varying")
            .HasColumnName("secret");

        builder.Property(e => e.Superapp).HasColumnName("superapp");

        builder.Property(e => e.Uid)
            .HasColumnType("character varying")
            .HasColumnName("uid");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Website)
            .HasColumnType("character varying")
            .HasColumnName("website");

        builder.HasOne(d => d.Owner)
            .WithMany(p => p.OAuthApplications)
            .HasForeignKey(d => d.OwnerId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_b0988c7c0a");
    }
}