namespace Smilodon.Domain.Models
{
    public class FollowRecommendation
    {
        public long? AccountId { get; set; }
        public decimal? Rank { get; set; }
        public string[]? Reason { get; set; }
    }
}
