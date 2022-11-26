using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class LoginActivityEntityConfiguration : IEntityTypeConfiguration<LoginActivity>
{
    public void Configure(EntityTypeBuilder<LoginActivity> builder)
    {
        builder.ToTable("login_activities");
        
        builder.HasKey(e => e.Id).HasName("login_activities_pkey");

        builder.HasIndex(e => e.UserId).HasDatabaseName("index_login_activities_on_user_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AuthenticationMethod)
            .HasColumnType("character varying")
            .HasColumnName("authentication_method");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.FailureReason)
            .HasColumnType("character varying")
            .HasColumnName("failure_reason");

        builder.Property(e => e.Ip).HasColumnName("ip");

        builder.Property(e => e.Provider)
            .HasColumnType("character varying")
            .HasColumnName("provider");

        builder.Property(e => e.Success).HasColumnName("success");

        builder.Property(e => e.UserAgent)
            .HasColumnType("character varying")
            .HasColumnName("user_agent");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.User)
            .WithMany(p => p.LoginActivities)
            .HasForeignKey(d => d.UserId)
            .HasConstraintName("fk_rails_e4b6396b41");
    }
}