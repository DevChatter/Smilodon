namespace Smilodon.Domain.Models
{
    public class ScheduledStatus
    {
        public long Id { get; set; }
        public long? AccountId { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public string? Params { get; set; }

        public virtual Account? Account { get; set; }
        public virtual ICollection<MediaAttachment> MediaAttachments { get; set; } = new HashSet<MediaAttachment>();
    }
}
