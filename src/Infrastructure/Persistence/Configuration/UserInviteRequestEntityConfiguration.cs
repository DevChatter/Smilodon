using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class UserInviteRequestEntityConfiguration : IEntityTypeConfiguration<UserInviteRequest>
{
    public void Configure(EntityTypeBuilder<UserInviteRequest> builder)
    {
        builder.ToTable("user_invite_requests");
        
        builder.HasKey(e => e.Id).HasName("user_invite_requests_pkey");

        builder.HasIndex(e => e.UserId).HasDatabaseName("index_user_invite_requests_on_user_id");

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.Text).HasColumnName("text");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("updated_at");

        builder.Property(e => e.UserId).HasColumnName("user_id");

        builder.HasOne(d => d.User)
            .WithMany(p => p.UserInviteRequests)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("fk_rails_3773f15361");
    }
}