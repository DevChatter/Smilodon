using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class ReportNoteEntityConfiguration : IEntityTypeConfiguration<ReportNote>
{
    public void Configure(EntityTypeBuilder<ReportNote> builder)
    {
        builder.ToTable("report_notes");
        
        builder.HasKey(e => e.Id).HasName("report_notes_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_report_notes_on_account_id");

        builder.HasIndex(e => e.ReportId).HasDatabaseName("index_report_notes_on_report_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Content).HasColumnName("content");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ReportId).HasColumnName("report_id");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.ReportNotes)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_rails_cae66353f3");

        builder.HasOne(d => d.Report)
            .WithMany(p => p.ReportNotes)
            .HasForeignKey(d => d.ReportId)
            .HasConstraintName("fk_rails_7fa83a61eb");
    }
}