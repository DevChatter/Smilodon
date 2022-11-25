using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class OAuthAccessGrantEntityConfiguration : IEntityTypeConfiguration<OAuthAccessGrant>
{
    public void Configure(EntityTypeBuilder<OAuthAccessGrant> builder)
    {
        builder.ToTable("oauth_access_grants");
        
        builder.HasKey(e => e.Id).HasName("oauth_access_grants_pkey");

        builder.HasIndex(e => e.ResourceOwnerId).HasDatabaseName("index_oauth_access_grants_on_resource_owner_id");

        builder.HasIndex(e => e.Token)
            .HasDatabaseName("index_oauth_access_grants_on_token")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.ApplicationId).HasColumnName("application_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ExpiresIn).HasColumnName("expires_in");

        builder.Property(e => e.RedirectUri).HasColumnName("redirect_uri");

        builder.Property(e => e.ResourceOwnerId).HasColumnName("resource_owner_id");

        builder.Property(e => e.RevokedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("revoked_at");

        builder.Property(e => e.Scopes)
            .HasColumnType("character varying")
            .HasColumnName("scopes");

        builder.Property(e => e.Token)
            .HasColumnType("character varying")
            .HasColumnName("token");

        builder.HasOne(d => d.Application)
            .WithMany(p => p.OAuthAccessGrants)
            .HasForeignKey(d => d.ApplicationId)
            .HasConstraintName("fk_34d54b0a33");

        builder.HasOne(d => d.ResourceOwner)
            .WithMany(p => p.OAuthAccessGrants)
            .HasForeignKey(d => d.ResourceOwnerId)
            .HasConstraintName("fk_63b044929b");
    }
}