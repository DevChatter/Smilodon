namespace Smilodon.Domain.Models
{
    public class Status
    {
        public long Id { get; set; }
        public string? Uri { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? InReplyToId { get; set; }
        public long? ReblogOfId { get; set; }
        public string? Url { get; set; }
        public bool Sensitive { get; set; }
        public int Visibility { get; set; }
        public string SpoilerText { get; set; } = null!;
        public bool Reply { get; set; }
        public string? Language { get; set; }
        public long? ConversationId { get; set; }
        public bool? Local { get; set; }
        public long AccountId { get; set; }
        public long? ApplicationId { get; set; }
        public long? InReplyToAccountId { get; set; }
        public long? PollId { get; set; }
        public DateTime? DeletedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public bool? Trendable { get; set; }
        public long[]? OrderedMediaAttachmentIds { get; set; }

        public virtual Account Account { get; set; } = null!;
        public virtual Status? InReplyTo { get; set; }
        public virtual Account? InReplyToAccount { get; set; }
        public virtual Status? ReblogOf { get; set; }
        public virtual StatusStat StatusStat { get; set; } = null!;
        public virtual StatusTrend StatusTrend { get; set; } = null!;
        public virtual ICollection<Bookmark> Bookmarks { get; set; } = new HashSet<Bookmark>();
        public virtual ICollection<CustomFilterStatus> CustomFilterStatuses { get; set; } = new HashSet<CustomFilterStatus>();
        public virtual ICollection<Favourite> Favourites { get; set; } = new HashSet<Favourite>();
        public virtual ICollection<Status> InverseInReplyTo { get; set; } = new HashSet<Status>();
        public virtual ICollection<Status> InverseReblogOf { get; set; } = new HashSet<Status>();
        public virtual ICollection<MediaAttachment> MediaAttachments { get; set; } = new HashSet<MediaAttachment>();
        public virtual ICollection<Mention> Mentions { get; set; } = new HashSet<Mention>();
        public virtual ICollection<Poll> Polls { get; set; } = new HashSet<Poll>();
        public virtual ICollection<StatusEdit> StatusEdits { get; set; } = new HashSet<StatusEdit>();
        public virtual ICollection<StatusPin> StatusPins { get; set; } = new HashSet<StatusPin>();
    }
}
