using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class ConversationEntityConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("conversations");
        
        builder.HasKey(e => e.Id).HasName("conversations_pkey");

        builder.HasIndex(e => e.Uri)
            .HasDatabaseName("index_conversations_on_uri")
            .IsUnique()
            .HasFilter("(uri IS NOT NULL)")
            .HasOperators("text_pattern_ops");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Uri)
            .HasColumnType("character varying")
            .HasColumnName("uri");
    }
}