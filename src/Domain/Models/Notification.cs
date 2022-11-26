namespace Smilodon.Domain.Models
{
    public class Notification
    {
        public long Id { get; set; }
        public long ActivityId { get; set; }
        public string ActivityType { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long AccountId { get; set; }
        public long FromAccountId { get; set; }
        public string? Type { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Account FromAccount { get; set; } = null!;
    }
}
