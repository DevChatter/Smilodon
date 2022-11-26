namespace Smilodon.Domain.Models
{
    public class AccountWarning
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public long? TargetAccountId { get; set; }
        public int Action { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? ReportId { get; set; }
        public string[]? StatusIds { get; set; }
        public DateTime? OverruledAt { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Report? Report { get; set; }
        public virtual Account? TargetAccount { get; set; }
        public virtual Appeal Appeal { get; set; } = null!;
    }
}
