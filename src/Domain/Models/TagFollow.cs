namespace Smilodon.Domain.Models
{
    public class TagFollow
    {
        public long Id { get; set; }
        public long TagId { get; set; }
        public long AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Tag Tag { get; set; } = null!;
    }
}
