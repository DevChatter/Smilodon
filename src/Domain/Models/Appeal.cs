namespace Smilodon.Domain.Models
{
    public class Appeal
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long AccountWarningId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime? ApprovedAt { get; set; }
        public long? ApprovedByAccountId { get; set; }
        public DateTime? RejectedAt { get; set; }
        public long? RejectedByAccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual AccountWarning AccountWarning { get; set; } = null!;
        public virtual Account? ApprovedByAccount { get; set; }
        public virtual Account? RejectedByAccount { get; set; }
    }
}
