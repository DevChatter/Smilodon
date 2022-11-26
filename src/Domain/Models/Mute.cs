namespace Smilodon.Domain.Models
{
    public class Mute
    {
        public long Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool HideNotifications { get; set; }
        public long AccountId { get; set; }
        public long TargetAccountId { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Account TargetAccount { get; set; } = null!;
    }
}
