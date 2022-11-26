using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class OAuthAccessTokenEntityConfiguration : IEntityTypeConfiguration<OAuthAccessToken>
{
    public void Configure(EntityTypeBuilder<OAuthAccessToken> builder)
    {
        builder.ToTable("oauth_access_tokens");
        
        builder.HasKey(e => e.Id).HasName("oauth_access_tokens_pkey");

        builder.HasIndex(e => e.RefreshToken)
            .HasDatabaseName("index_oauth_access_tokens_on_refresh_token")
            .IsUnique()
            .HasFilter("(refresh_token IS NOT NULL)")
            .HasOperators(new[] { "text_pattern_ops" });

        builder.HasIndex(e => e.ResourceOwnerId)
            .HasDatabaseName("index_oauth_access_tokens_on_resource_owner_id")
            .HasFilter("(resource_owner_id IS NOT NULL)");

        builder.HasIndex(e => e.Token)
            .HasDatabaseName("index_oauth_access_tokens_on_token")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.ApplicationId).HasColumnName("application_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ExpiresIn).HasColumnName("expires_in");

        builder.Property(e => e.LastUsedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("last_used_at");

        builder.Property(e => e.LastUsedIp).HasColumnName("last_used_ip");

        builder.Property(e => e.RefreshToken)
            .HasColumnType("character varying")
            .HasColumnName("refresh_token");

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
            .WithMany(p => p.OAuthAccessTokens)
            .HasForeignKey(d => d.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_f5fc4c1ee3");

        builder.HasOne(d => d.ResourceOwner)
            .WithMany(p => p.OAuthAccessTokens)
            .HasForeignKey(d => d.ResourceOwnerId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_e84df68546");
    }
}