using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class WebSettingEntityConfiguration : IEntityTypeConfiguration<WebSetting>
{
    public void Configure(EntityTypeBuilder<WebSetting> builder)
    {
        builder.ToTable("web_settings");
        
        builder.HasKey(e => e.Id).HasName("web_settings_pkey");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("index_web_settings_on_user_id")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Data)
            .HasColumnType("json")
            .HasColumnName("data");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.User)
            .WithOne(p => p.WebSetting)
            .HasForeignKey<WebSetting>(d => d.UserId)
            .HasConstraintName("fk_11910667b2");
    }
}