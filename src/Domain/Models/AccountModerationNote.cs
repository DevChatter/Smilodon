namespace Smilodon.Domain.Models
{
    public class AccountModerationNote
    {
        public long Id { get; set; }
        public string Content { get; set; } = null!;
        public long AccountId { get; set; }
        public long TargetAccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Account TargetAccount { get; set; } = null!;
    }
}
