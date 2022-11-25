namespace Smilodon.Domain.Models
{
    public class AccountStat
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long StatusesCount { get; set; }
        public long FollowingCount { get; set; }
        public long FollowersCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastStatusAt { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
