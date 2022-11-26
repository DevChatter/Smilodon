using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class FollowRecommendationEntityConfiguration : IEntityTypeConfiguration<FollowRecommendation>
{
    public void Configure(EntityTypeBuilder<FollowRecommendation> builder)
    {
        builder.HasNoKey();

        builder.ToView("follow_recommendations");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.Rank).HasColumnName("rank");

        builder.Property(e => e.Reason).HasColumnName("reason");
    }
}