namespace Smilodon.Domain.Models
{
    public class AccountNote
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public long? TargetAccountId { get; set; }
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Account? TargetAccount { get; set; }
    }
}
