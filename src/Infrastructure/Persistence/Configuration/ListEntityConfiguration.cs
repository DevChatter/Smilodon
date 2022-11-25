using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class ListEntityConfiguration : IEntityTypeConfiguration<List>
{
    public void Configure(EntityTypeBuilder<List> builder)
    {
        builder.ToTable("lists");
        
        builder.HasKey(e => e.Id).HasName("lists_pkey");

        builder.HasIndex(e => e.AccountId).HasDatabaseName("index_lists_on_account_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.RepliesPolicy).HasColumnName("replies_policy");

        builder.Property(e => e.Title)
            .HasColumnType("character varying")
            .HasColumnName("title")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithMany(p => p.Lists)
            .HasForeignKey(d => d.AccountId)
            .HasConstraintName("fk_rails_3853b78dac");
    }
}