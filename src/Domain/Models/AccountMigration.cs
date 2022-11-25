namespace Smilodon.Domain.Models
{
    public class AccountMigration
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public string Acct { get; set; } = null!;
        public long FollowersCount { get; set; }
        public long? TargetAccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Account? TargetAccount { get; set; }
    }
}
