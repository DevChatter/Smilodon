namespace Smilodon.Domain.Models
{
    public class FeaturedTag
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long TagId { get; set; }
        public long StatusesCount { get; set; }
        public DateTime? LastStatusAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Name { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
