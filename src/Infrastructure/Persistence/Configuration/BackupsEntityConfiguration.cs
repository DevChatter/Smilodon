using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class BackupsEntityConfiguration : IEntityTypeConfiguration<Backup>
{
    public void Configure(EntityTypeBuilder<Backup> builder)
    {
        builder.ToTable("backups");
        
        builder.HasKey(e => e.Id).HasName("backups_pkey");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.DumpContentType)
            .HasColumnType("character varying")
            .HasColumnName("dump_content_type");

        builder.Property(e => e.DumpFileName)
            .HasColumnType("character varying")
            .HasColumnName("dump_file_name");

        builder.Property(e => e.DumpFileSize).HasColumnName("dump_file_size");

        builder.Property(e => e.DumpUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("dump_updated_at");

        builder.Property(e => e.Processed).HasColumnName("processed");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.User)
            .WithMany(p => p.Backups)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("fk_rails_096669d221");
    }
}