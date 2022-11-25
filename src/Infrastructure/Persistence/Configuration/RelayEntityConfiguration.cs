using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class RelayEntityConfiguration : IEntityTypeConfiguration<Relay>
{
    public void Configure(EntityTypeBuilder<Relay> builder)
    {
        builder.ToTable("relays");
        
        builder.HasKey(e => e.Id).HasName("relays_pkey");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.FollowActivityId)
            .HasColumnType("character varying")
            .HasColumnName("follow_activity_id");

        builder.Property(e => e.InboxUrl)
            .HasColumnType("character varying")
            .HasColumnName("inbox_url")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.State).HasColumnName("state");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");
    }
}