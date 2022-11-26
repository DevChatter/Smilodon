namespace Smilodon.Domain.Models
{
    public class FollowRecommendationSuppression
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
