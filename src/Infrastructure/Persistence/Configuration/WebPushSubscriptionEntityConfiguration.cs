using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class WebPushSubscriptionEntityConfiguration : IEntityTypeConfiguration<WebPushSubscription>
{
    public void Configure(EntityTypeBuilder<WebPushSubscription> builder)
    {
        builder.ToTable("web_push_subscriptions");
        
        builder.HasKey(e => e.Id).HasName("web_push_subscriptions_pkey");

        builder.HasIndex(e => e.AccessTokenId)
            .HasDatabaseName("index_web_push_subscriptions_on_access_token_id")
            .HasFilter("(access_token_id IS NOT NULL)");

        builder.HasIndex(e => e.UserId).HasDatabaseName("index_web_push_subscriptions_on_user_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccessTokenId).HasColumnName("access_token_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Data)
            .HasColumnType("json")
            .HasColumnName("data");

        builder.Property(e => e.Endpoint)
            .HasColumnType("character varying")
            .HasColumnName("endpoint");

        builder.Property(e => e.KeyAuth)
            .HasColumnType("character varying")
            .HasColumnName("key_auth");

        builder.Property(e => e.KeyP256dh)
            .HasColumnType("character varying")
            .HasColumnName("key_p256dh");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.AccessToken)
            .WithMany(p => p.WebPushSubscriptions)
            .HasForeignKey(d => d.AccessTokenId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_751a9f390b");

        builder.HasOne(d => d.User)
            .WithMany(p => p.WebPushSubscriptions)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_b006f28dac");
    }
}