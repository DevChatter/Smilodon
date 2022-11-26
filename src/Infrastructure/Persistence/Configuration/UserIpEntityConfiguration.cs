using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class UserIpEntityConfiguration : IEntityTypeConfiguration<UserIp>
{
    public void Configure(EntityTypeBuilder<UserIp> builder)
    {
        builder.HasNoKey();

        builder.ToView("user_ips");

        builder.Property(e => e.Ip).HasColumnName("ip");

        builder.Property(e => e.UsedAt)
            .HasColumnType("timestamp without time zone")
            .HasColumnName("used_at");

        builder.Property(e => e.UserId).HasColumnName("user_id");
    }
}