using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class WebAuthnCredentialEntityConfiguration : IEntityTypeConfiguration<WebAuthnCredential>
{
    public void Configure(EntityTypeBuilder<WebAuthnCredential> builder)
    {
        builder.ToTable("webauthn_credentials");
        
        builder.HasKey(e => e.Id).HasName("webauthn_credentials_pkey");

        builder.HasIndex(e => e.ExternalId)
            .HasDatabaseName("index_webauthn_credentials_on_external_id")
            .IsUnique();

        builder.HasIndex(e => e.UserId).HasDatabaseName("index_webauthn_credentials_on_user_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ExternalId)
            .HasColumnType("character varying")
            .HasColumnName("external_id");

        builder.Property(e => e.Nickname)
            .HasColumnType("character varying")
            .HasColumnName("nickname");

        builder.Property(e => e.PublicKey)
            .HasColumnType("character varying")
            .HasColumnName("public_key");

        builder.Property(e => e.SignCount).HasColumnName("sign_count");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.User)
            .WithMany(p => p.WebAuthnCredentials)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_rails_a4355aef77");
    }
}