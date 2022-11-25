using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class MentionEntityConfiguration : IEntityTypeConfiguration<Mention>
{
    public void Configure(EntityTypeBuilder<Mention> builder)
    {
        builder.ToTable("mentions");
        
        builder.HasKey(e => e.Id).HasName("mentions_pkey");

        builder.HasIndex(e => new { e.AccountId, e.StatusId })
            .HasDatabaseName("index_mentions_on_account_id_and_status_id")
            .IsUnique();

        builder.HasIndex(e => e.StatusId).HasDatabaseName("index_mentions_on_status_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Silent).HasColumnName("silent");

        builder.Property(e => e.StatusId).HasColumnName("status_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.Mentions)
            .HasForeignKey(d => d.AccountId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_970d43f9d1");

        builder.HasOne(d => d.Status)
            .WithMany(p => p.Mentions)
            .HasForeignKey(d => d.StatusId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_59edbe2887");
    }
}