namespace Smilodon.Domain.Models
{
    public class Report
    {
        public long Id { get; set; }
        public long[] StatusIds { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long AccountId { get; set; }
        public long? ActionTakenByAccountId { get; set; }
        public long TargetAccountId { get; set; }
        public long? AssignedAccountId { get; set; }
        public string? Uri { get; set; }
        public bool? Forwarded { get; set; }
        public int Category { get; set; }
        public DateTime? ActionTakenAt { get; set; }
        public long[]? RuleIds { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Account? ActionTakenByAccount { get; set; }
        public virtual Account? AssignedAccount { get; set; }
        public virtual Account TargetAccount { get; set; } = null!;
        public virtual ICollection<AccountWarning> AccountWarnings { get; set; } =  new HashSet<AccountWarning>();
        public virtual ICollection<ReportNote> ReportNotes { get; set; } = new HashSet<ReportNote>();
    }
}
