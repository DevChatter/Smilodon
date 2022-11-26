using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class SettingEntityConfiguration : IEntityTypeConfiguration<Setting>
{
    public void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable("settings");
        
        builder.HasKey(e => e.Id).HasName("settings_pkey");

        builder.HasIndex(e => new { e.ThingType, e.ThingId, e.Var })
            .HasDatabaseName("index_settings_on_thing_type_and_thing_id_and_var")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.ThingId).HasColumnName("thing_id");

        builder.Property(e => e.ThingType)
            .HasColumnType("character varying")
            .HasColumnName("thing_type");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.Value).HasColumnName("value");

        builder.Property(e => e.Var)
            .HasColumnType("character varying")
            .HasColumnName("var");
    }
}