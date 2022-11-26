namespace Smilodon.Domain.Models
{
    public class StatusEdit
    {
        public long Id { get; set; }
        public long StatusId { get; set; }
        public long? AccountId { get; set; }
        public string Text { get; set; } = null!;
        public string SpoilerText { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long[]? OrderedMediaAttachmentIds { get; set; }
        public string[]? MediaDescriptions { get; set; }
        public string[]? PollOptions { get; set; }
        public bool? Sensitive { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Status Status { get; set; } = null!;
    }
}
