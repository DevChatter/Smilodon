using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class EncryptedMessageEntityConfiguration : IEntityTypeConfiguration<EncryptedMessage>
{
    public void Configure(EntityTypeBuilder<EncryptedMessage> builder)
    {
        builder.ToTable("encrypted_messages");
        
        builder.HasKey(e => e.Id).HasName("encrypted_messages_pkey");

        builder.HasIndex(e => e.DeviceId).HasDatabaseName("index_encrypted_messages_on_device_id");

        builder.HasIndex(e => e.FromAccountId).HasDatabaseName("index_encrypted_messages_on_from_account_id");

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("timestamp_id('encrypted_messages'::text)");

        builder.Property(e => e.Body)
            .HasColumnName("body")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.DeviceId).HasColumnName("device_id");

        builder.Property(e => e.Digest)
            .HasColumnName("digest")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.FromAccountId).HasColumnName("from_account_id");

        builder.Property(e => e.FromDeviceId)
            .HasColumnType("character varying")
            .HasColumnName("from_device_id")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.MessageFranking)
            .HasColumnName("message_franking")
            .HasDefaultValueSql("''::text");

        builder.Property(e => e.Type).HasColumnName("type");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Device)
            .WithMany(p => p.EncryptedMessages)
            .HasForeignKey(d => d.DeviceId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_a83e4df7ae");

        builder.HasOne(d => d.FromAccount)
            .WithMany(p => p.EncryptedMessages)
            .HasForeignKey(d => d.FromAccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_a42ad0f8d5");
    }
}