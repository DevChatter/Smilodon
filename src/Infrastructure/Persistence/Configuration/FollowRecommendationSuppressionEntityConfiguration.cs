using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Smilodon.Domain.Models;

namespace Smilodon.Infrastructure.Persistence.Configuration;

public class
    FollowRecommendationSuppressionEntityConfiguration : IEntityTypeConfiguration<FollowRecommendationSuppression>
{
    public void Configure(EntityTypeBuilder<FollowRecommendationSuppression> builder)
    {
        builder.ToTable("follow_recommendation_suppressions");
        
        builder.HasKey(e => e.Id).HasName("follow_recommendation_suppressions_pkey");

        builder.HasIndex(e => e.AccountId)
            .HasDatabaseName("index_follow_recommendation_suppressions_on_account_id")
            .IsUnique();

        builder.Property(e => e.Id).HasColumnName("id");

        builder.Property(e => e.AccountId).HasColumnName("account_id");

        builder.Property(e => e.CreatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("created_at");

        builder.Property(e => e.UpdatedAt)
            .HasColumnType("timestamp(6) without time zone")
            .HasColumnName("updated_at");

        builder.HasOne(d => d.Account)
            .WithOne(p => p.FollowRecommendationSuppression)
            .HasForeignKey<FollowRecommendationSuppression>(d => d.AccountId)
            .HasConstraintName("fk_rails_dfb9a1dbe2");
    }
}