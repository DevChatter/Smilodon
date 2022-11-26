namespace Smilodon.Domain.Models
{
    public class Announcement
    {
        public long Id { get; set; }
        public string Text { get; set; } = null!;
        public bool Published { get; set; }
        public bool AllDay { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? StartsAt { get; set; }
        public DateTime? EndsAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public long[]? StatusIds { get; set; }

        public virtual ICollection<AnnouncementMute> AnnouncementMutes { get; set; } = new HashSet<AnnouncementMute>();
        public virtual ICollection<AnnouncementReaction> AnnouncementReactions { get; set; } = new HashSet<AnnouncementReaction>();
    }
}
