using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class SessionActivationEntityConfiguration : IEntityTypeConfiguration<SessionActivation>
{
    public void Configure(EntityTypeBuilder<SessionActivation> builder)
    {
        builder.ToTable("session_activations");
        
        builder.HasKey(e => e.Id).HasName("session_activations_pkey");

        builder.HasIndex(e => e.AccessTokenId).HasDatabaseName("index_session_activations_on_access_token_id");

        builder.HasIndex(e => e.SessionId)
            .HasDatabaseName("index_session_activations_on_session_id")
            .IsUnique();

        builder.HasIndex(e => e.UserId).HasDatabaseName("index_session_activations_on_user_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccessTokenId).HasColumnName("access_token_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Ip).HasColumnName("ip");

        builder.Property(e => e.SessionId)
            .HasColumnType("character varying")
            .HasColumnName("session_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.UserAgent)
            .HasColumnType("character varying")
            .HasColumnName("user_agent")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.Property(e => e.WebPushSubscriptionId).HasColumnName("web_push_subscription_id");

        builder.HasOne(d => d.AccessToken)
            .WithMany(p => p.SessionActivations)
            .HasForeignKey(d => d.AccessTokenId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_957e5bda89");

        builder.HasOne(d => d.User)
            .WithMany(p => p.SessionActivations)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_e5fda67334");
    }
}