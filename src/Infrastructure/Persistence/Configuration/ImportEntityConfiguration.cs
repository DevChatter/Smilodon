using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class ImportEntityConfiguration : IEntityTypeConfiguration<Import>
{
    public void Configure(EntityTypeBuilder<Import> builder)
    {
        builder.ToTable("imports");
        
        builder.HasKey(e => e.Id).HasName("imports_pkey");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Approved).HasColumnName("approved");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.DataContentType)
            .HasColumnType("character varying")
            .HasColumnName("data_content_type");

        builder.Property(e => e.DataFileName)
            .HasColumnType("character varying")
            .HasColumnName("data_file_name");

        builder.Property(e => e.DataFileSize).HasColumnName("data_file_size");

        builder.Property(e => e.DataUpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("data_updated_at");

        builder.Property(e => e.Overwrite).HasColumnName("overwrite");

        builder.Property(e => e.Type).HasColumnName("type");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.Imports)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_6db1b6e408");
    }
}