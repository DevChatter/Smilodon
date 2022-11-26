using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("user_roles");
        
        builder.HasKey(e => e.Id).HasName("user_roles_pkey");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.Color)
            .HasColumnType("character varying")
            .HasColumnName("color")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Highlighted).HasColumnName("highlighted");

        builder.Property(e => e.Name)
            .HasColumnType("character varying")
            .HasColumnName("name")
            .HasDefaultValueSql("''::character varying");

        builder.Property(e => e.Permissions).HasColumnName("permissions");

        builder.Property(e => e.Position).HasColumnName("position");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");
    }
}